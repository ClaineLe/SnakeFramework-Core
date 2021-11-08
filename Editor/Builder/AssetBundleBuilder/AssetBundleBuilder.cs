using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Build.Pipeline;
using com.snake.framework.runtime;

namespace com.snake.framework
{
    namespace editor
    {
        /// <summary>
        /// AssetBundle构建者
        /// </summary>
        public class AssetBundleBuilder
        {
            /// <summary>
            /// 构建AssetBundle
            /// </summary>
            /// <param name="outputPath"></param>
            /// <param name="buildTarget"></param>
            /// <param name="resVersion"></param>
            /// <param name="callback"></param>
            static public void BuildAssetBundle(BundleBuildParameters parameters, Action<AssetBundleCatalog> callback)
            {
                string outputPath = parameters.OutputFolder;
                BuildTarget buildTarget = parameters.Target;

                var assetBundleCatalog = new AssetBundleCatalog();
                if (Directory.Exists(outputPath))
                {
                    Directory.Delete(outputPath, true);
                }
                Directory.CreateDirectory(outputPath);
                var builds = generateAssetBundleList().ToArray();
                var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
                var content = new BundleBuildContent(builds);
                var exitCode = ContentPipeline.BuildAssetBundles(parameters, content, out var results);
                if (exitCode < ReturnCode.Success)
                {
                    SnakeDebuger.Error("构建资源包错误. code:" + exitCode);
                    callback?.Invoke(null);
                    return;
                }

                foreach (var a in Directory.GetFiles(outputPath, "*.manifest"))
                    File.Delete(a);
                foreach (var a in Directory.GetFiles(outputPath, "*.json"))
                    File.Delete(a);

                generateAssetBundleCatalog(builds, results.BundleInfos, ref assetBundleCatalog);
                callback?.Invoke(assetBundleCatalog);
            }
            static private Dictionary<string, string> genAssetMap(AssetRule assetRule)
            {
                Dictionary<string, string> assetMap = new Dictionary<string, string>();

                DirectoryInfo dirInfo = new DirectoryInfo(assetRule.foldPath);
                List<FileInfo> fileInfoList = new List<FileInfo>();

                //收集
                for (int i = 0; i < assetRule.types.Length; i++)
                    fileInfoList.AddRange(dirInfo.GetFiles(assetRule.types[i], SearchOption.AllDirectories));

                //过滤
                foreach (string filterStr in assetRule.filters)
                    fileInfoList.RemoveAll(a => a.FullName.Contains(filterStr));

                string bundleName = dirInfo.FullName.Replace("\\", "/").Replace(Application.dataPath + "/ResExport/", string.Empty)
                  .Replace("/", "_").Replace(".", "_").ToLower();

                for (var i = 0; i < fileInfoList.Count; i++)
                {
                    FileInfo item = fileInfoList[i];
                    string fileFullPath = item.FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                    if (assetRule.single)
                    {
                        bundleName = item.FullName.Replace("\\", "/").Replace(Application.dataPath + "/ResExport/", string.Empty);
                        bundleName = Path.Combine(Path.GetDirectoryName(bundleName), Path.GetFileNameWithoutExtension(bundleName)).Replace("\\", "/");
                        bundleName = bundleName.Replace("/", "_").Replace(".", "_").ToLower();
                    }

                    assetMap.Add(fileFullPath, bundleName);
                }
                return assetMap;
            }
            static private Dictionary<string, string> generateAssetMap()
            {
                var assetMapDict = new Dictionary<string, string>();
                BuilderSetting builderSetting = BuilderSetting.EditorGet();
                if (string.IsNullOrEmpty(builderSetting.mAssetRulesPath))
                {
                    SnakeDebuger.Error("未在EnvironmentSetting.asset中配置，资源规则路径.");
                    return null;
                }
                string[] files = Directory.GetFiles(builderSetting.mAssetRulesPath, "*.asset");

                string tmpPath = string.Empty;
                Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                for (int i = 0; i < files.Length; i++)
                {
                    tmpPath = files[i].Replace("\\", "/");
                    AssetRule assetRule = AssetDatabase.LoadAssetAtPath<AssetRule>(tmpPath);
                    if (assetRule == null)
                    {
                        SnakeDebuger.Error("没有找到AssetRule.Path:" + tmpPath);
                        continue;
                    }

                    tmpDict = genAssetMap(assetRule);
                    if (tmpDict.Count <= 0)
                        continue;

                    Dictionary<string, string>.Enumerator enumerator = tmpDict.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (assetMapDict.TryGetValue(enumerator.Current.Key, out string bundleName) == true)
                            continue;
                        assetMapDict.Add(enumerator.Current.Key, enumerator.Current.Value);
                    }
                }
                return assetMapDict;
            }
            static private List<AssetBundleBuild> generateAssetBundleList()
            {
                Dictionary<string, string> mapping = generateAssetMap();
                Dictionary<string, List<string>> assetBundleInfoDic = new Dictionary<string, List<string>>();
                foreach (var a in mapping)
                {
                    if (assetBundleInfoDic.TryGetValue(a.Value, out var assetList) == false)
                    {
                        assetList = new List<string>();
                        assetBundleInfoDic.Add(a.Value, assetList);
                    }
                    assetList.Add(a.Key);
                }

                List<AssetBundleBuild> assetBundleBuildList = new List<AssetBundleBuild>();
                foreach (var a in assetBundleInfoDic)
                {
                    assetBundleBuildList.Add(new AssetBundleBuild()
                    {
                        assetBundleName = a.Key,
                        assetNames = a.Value.ToArray()
                    });
                }
                return assetBundleBuildList;
            }
            static private void generateAssetBundleCatalog(AssetBundleBuild[] builds, Dictionary<string, BundleDetails> bundleInfos, ref AssetBundleCatalog assetBundleCatalog)
            {
                foreach (AssetBundleBuild a in builds)
                {
                    foreach (string b in a.assetNames)
                    {
                        assetBundleCatalog.mPathMapping.Add(b, "assetbundle/" + a.assetBundleName);
                    }
                    BundleDetails bundleDetails;
                    if (bundleInfos.TryGetValue(a.assetBundleName, out bundleDetails))
                    {
                        var dependList = new List<string>();
                        getDependends(a.assetBundleName, bundleInfos, ref dependList);
                        string[] depends = new string[0];
                        if (dependList != null && dependList.Count > 0)
                        {
                            depends = dependList.Distinct().ToArray();
                            for (var i = 0; i < depends.Length; i++)
                            {
                                depends[i] = $"assetbundle/{depends[i]}";
                            }
                        }
                        assetBundleCatalog.mDepensMapping.Add("assetbundle/" + a.assetBundleName, depends);
                    }
                }
            }
            static private void getDependends(string main, Dictionary<string, BundleDetails> bundleInfos, ref List<string> list)
            {
                var result = bundleInfos.TryGetValue(main, out var details);
                if (!result)
                {
                    return;
                }
                if (details.Dependencies == null || details.Dependencies.Length == 0)
                {
                    return;
                }
                for (var i = 0; i < details.Dependencies.Length; i++)
                {
                    var depend = details.Dependencies[i];
                    if (list.Contains(depend))
                    {
                        continue;
                    }
                    list.Add(depend);
                    getDependends(depend, bundleInfos, ref list);
                }
            }
        }
    }
}