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
            static public void BuildAssetBundle(SnakeBuildBundleOptions buildBundleOptions, Action<AssetBundleCatalog> callback)
            {
                string outputPath = buildBundleOptions.mParameters.OutputFolder;
                BuildTarget buildTarget = buildBundleOptions.mParameters.Target;

                var assetBundleCatalog = new AssetBundleCatalog();
                if (Directory.Exists(outputPath))
                {
                    Directory.Delete(outputPath, true);
                }
                Directory.CreateDirectory(outputPath);
                AssetBundleBuild[] builds = generateAssetBundleList().ToArray();
                BundleBuildContent content = new BundleBuildContent(builds);
                ReturnCode exitCode = ContentPipeline.BuildAssetBundles(buildBundleOptions.mParameters, content, out var results);
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

                if (string.IsNullOrEmpty(buildBundleOptions.mExtOutPutPath) == false && buildBundleOptions.mUsingAssets.Length > 0)
                {
                    List<string> usingBundleNameList = new List<string>();
                    int index = 0;
                    foreach (AssetBundleBuild assetBundleBuild in builds)
                    {
                        foreach (string assetName in assetBundleBuild.assetNames)
                        {
                            index = Array.FindIndex(buildBundleOptions.mUsingAssets, array => array == assetName);
                            if (index < 0)
                                continue;
                            usingBundleNameList.Add(assetBundleBuild.assetBundleName);
                        }
                    }
                    usingBundleNameList = usingBundleNameList.Distinct().ToList();
                    foreach (var bundleName in usingBundleNameList)
                    {
                        string formPath = Path.Combine(outputPath, bundleName);
                        string toPath = Path.Combine(buildBundleOptions.mExtOutPutPath, bundleName);
                        FileInfo toFileInfo = new FileInfo(toPath);
                        if (toFileInfo.Directory.Exists == false)
                            toFileInfo.Directory.Create();
                        File.Move(formPath, toPath);
                    }
                }
                callback?.Invoke(assetBundleCatalog);
            }

            static private Dictionary<string, string> genAssetMap(AssetRule assetRule, SearchOption searchOpt = SearchOption.AllDirectories)
            {
                Dictionary<string, string> assetMap = new Dictionary<string, string>();

                DirectoryInfo dirInfo = new DirectoryInfo(assetRule.foldPath);
                List<FileSystemInfo> fileInfoList = new List<FileSystemInfo>();

                if (assetRule.packerMode == PACKER_MODE.childfold)
                {
                    if (assetRule.types == null || assetRule.types.Length == 0)
                    {
                        fileInfoList.AddRange(dirInfo.GetDirectories("*", SearchOption.AllDirectories));
                    }
                    else
                    {
                        for (int i = 0; i < assetRule.types.Length; i++)
                            fileInfoList.AddRange(dirInfo.GetDirectories(assetRule.types[i], SearchOption.AllDirectories));
                    }

                    foreach (var a in fileInfoList)
                    {
                        AssetRule childAssetRule = ScriptableObject.CreateInstance<AssetRule>();
                        childAssetRule.foldPath = a.FullName;
                        childAssetRule.packerMode = PACKER_MODE.together;
                        childAssetRule.types = assetRule.types;
                        childAssetRule.filters = assetRule.filters;
                        Dictionary<string, string> tmpDict = genAssetMap(childAssetRule, SearchOption.TopDirectoryOnly);
                        foreach (var b in tmpDict)
                        {
                            assetMap.Add(b.Key, b.Value);
                        }
                    }

                    return assetMap;
                }
                else
                {
                    if (assetRule.types == null || assetRule.types.Length == 0)
                    {
                        fileInfoList.AddRange(dirInfo.GetFiles("*.*", searchOpt));
                    }
                    else
                    {
                        for (int i = 0; i < assetRule.types.Length; i++)
                            fileInfoList.AddRange(dirInfo.GetFiles(assetRule.types[i], searchOpt));
                    }
                }

                //过滤
                foreach (string filterStr in assetRule.filters)
                    fileInfoList.RemoveAll(a => a.FullName.Contains(filterStr));

                string bundleName = GetFixPathString(dirInfo.FullName).Replace("/", "_").Replace(".", "_").ToLower();
                for (var i = 0; i < fileInfoList.Count; i++)
                {
                    FileSystemInfo item = fileInfoList[i];
                    string fileFullPath = item.FullName;
                    if (fileFullPath.Contains("\\Packages\\"))
                    {
                        var index = fileFullPath.IndexOf("Packages\\");
                        fileFullPath = fileFullPath.Substring(index);
                        fileFullPath = fileFullPath.Replace("\\", "/");
                    }
                    else
                    {
                        fileFullPath = fileFullPath.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                    }

                    if (assetRule.packerMode == PACKER_MODE.single)
                    {
                        bundleName = GetFixPathString(item.FullName);
                        bundleName = Path.Combine(Path.GetDirectoryName(bundleName), Path.GetFileNameWithoutExtension(bundleName)).Replace("\\", "/");
                        bundleName = bundleName.Replace("/", "_").Replace(".", "_").ToLower();
                    }
                    assetMap.Add(fileFullPath, bundleName);
                }
                return assetMap;
            }

            static private string GetFixPathString(string fullName)
            {
                var resultStr = "";
                if (fullName.Contains("\\Packages\\"))
                {
                    var index = fullName.IndexOf("Packages\\"); 

                    resultStr = fullName.Substring(index);
                    resultStr = resultStr.Replace("\\", "/");
                }
                else
                {
                    resultStr = fullName.Replace("\\", "/").Replace(Application.dataPath + "/ResExport/", string.Empty);
                }

                return resultStr;
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
                List<AssetRule> ruleList = new List<AssetRule>();

                string tmpPath = string.Empty;
                for (int i = 0; i < files.Length; i++)
                {
                    tmpPath = files[i].Replace("\\", "/");
                    AssetRule assetRule = AssetDatabase.LoadAssetAtPath<AssetRule>(tmpPath);
                    if (assetRule == null)
                    {
                        SnakeDebuger.Error("没有找到AssetRule.Path:" + tmpPath);
                        continue;
                    }

                    ruleList.Add(assetRule);
                }

                ruleList.Sort((a, b) => a.priority.CompareTo(b.priority));
                Dictionary<string, string> tmpDict = new Dictionary<string, string>();
                for (int i = 0; i < ruleList.Count; i++)
                {
                    var assetRule = ruleList[i];
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