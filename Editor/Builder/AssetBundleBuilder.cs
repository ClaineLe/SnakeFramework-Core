using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Build.Pipeline;
using com.snake.framework.runtime;
using UnityEditor.Build.Pipeline.Interfaces;

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
            static public void BuildAssetBundle(string assetBundleOutputPath, BuildTarget buildTarget, AssetBundleBuild[] assetBundleBuilds, Action<AssetBundleCatalog> callback, string extBundleOutputPath = null)
            {
                BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

                BuilderSetting setting = BuilderSetting.EditorGet();

                BundleBuildParameters parameters = new BundleBuildParameters(buildTarget, buildTargetGroup, setting.mBundleCachePath);

                var assetBundleCatalog = new AssetBundleCatalog();
                if (Directory.Exists(setting.mBundleCachePath))
                    Directory.Delete(setting.mBundleCachePath, true);

                Directory.CreateDirectory(setting.mBundleCachePath);
                BundleBuildContent content = new BundleBuildContent(assetBundleBuilds);
                ReturnCode exitCode = ContentPipeline.BuildAssetBundles(parameters, content, out IBundleBuildResults results);
                if (exitCode < ReturnCode.Success)
                {
                    SnakeDebuger.Error("构建资源包错误. code:" + exitCode);
                    callback?.Invoke(null);
                    return;
                }

                foreach (var a in Directory.GetFiles(setting.mBundleCachePath, "*.manifest"))
                    File.Delete(a);
                foreach (var a in Directory.GetFiles(setting.mBundleCachePath, "*.json"))
                    File.Delete(a);

                generateAssetBundleCatalog(assetBundleBuilds, results.BundleInfos, ref assetBundleCatalog);

                sourcePostProcessing(setting, extBundleOutputPath, assetBundleBuilds, results);

                callback?.Invoke(assetBundleCatalog);
            }

            static private void sourcePostProcessing(BuilderSetting setting, string extBundleOutputPath, AssetBundleBuild[] builds, IBundleBuildResults results)
            {
                string defBundleOutputPath = Path.Combine(Application.streamingAssetsPath, setting.mBundleCachePath);
                //清理默认资源目录
                if (Directory.Exists(defBundleOutputPath))
                {
                    Directory.Delete(defBundleOutputPath, true);
                    Directory.CreateDirectory(defBundleOutputPath);
                }

                //清理拓展资源目录
                if (Directory.Exists(extBundleOutputPath))
                {
                    Directory.Delete(extBundleOutputPath, true);
                    Directory.CreateDirectory(extBundleOutputPath);
                }

                if (string.IsNullOrEmpty(extBundleOutputPath) == true)
                {
                    //没有拓展资源，就把所有资源放到StreamingAssets下
                    foreach (var iter in results.BundleInfos)
                    {
                        string formPath = Path.Combine(setting.mBundleCachePath, iter.Key);
                        string toPath = Path.Combine(defBundleOutputPath, iter.Key);
                        FileInfo toFileInfo = new FileInfo(toPath);
                        if (toFileInfo.Directory.Exists == false)
                            toFileInfo.Directory.Create();
                        File.Move(formPath, toPath);
                    }
                }
                else
                {
                    //读取默认资源录制文件
                    List<string> defAssetList = new List<string>();
                    if (string.IsNullOrEmpty(setting.mDefAssetRecordFile) == false)
                    {
                        string[] assetPaths = System.IO.File.ReadAllLines(setting.mDefAssetRecordFile);
                        foreach (var assetPath in assetPaths)
                        {
                            if (string.IsNullOrEmpty(assetPath) == true)
                                continue;
                            defAssetList.Add(assetPath);
                        }
                    }

                    if (defAssetList.Count == 0)
                    {
                        //没有录制基础资源，所有资源放拓展资源目录下
                        foreach (var iter in results.BundleInfos)
                        {
                            string formPath = Path.Combine(setting.mBundleCachePath, iter.Key);
                            string toPath = Path.Combine(extBundleOutputPath, iter.Key);
                            FileInfo toFileInfo = new FileInfo(toPath);
                            if (toFileInfo.Directory.Exists == false)
                                toFileInfo.Directory.Create();
                            File.Move(formPath, toPath);
                        }
                    }
                    else
                    {
                        //将录制的基础资源，转换为BundleName
                        List<string> defBundleList = new List<string>();
                        int index = 0;
                        foreach (AssetBundleBuild assetBundleBuild in builds)
                        {
                            foreach (string assetName in assetBundleBuild.assetNames)
                            {
                                index = defAssetList.FindIndex(a => a == assetName);
                                if (index < 0)
                                    continue;
                                defBundleList.Add(assetBundleBuild.assetBundleName);
                            }
                        }
                        defBundleList = defBundleList.Distinct().ToList();

                        ///大小包分割逻辑
                        foreach (var iter in results.BundleInfos)
                        {
                            string formPath = Path.Combine(setting.mBundleCachePath, iter.Key);

                            //核心逻辑，判断是否为默认Bundle资源，确定复制到路径
                            index = defBundleList.FindIndex(a => a.Equals(iter.Key));
                            string toPath = Path.Combine(index >= 0 ? defBundleOutputPath : extBundleOutputPath, iter.Key);

                            //复制
                            FileInfo toFileInfo = new FileInfo(toPath);
                            if (toFileInfo.Directory.Exists == false)
                                toFileInfo.Directory.Create();
                            File.Copy(formPath, toPath);
                        }
                    }
                }
                AssetDatabase.Refresh();
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