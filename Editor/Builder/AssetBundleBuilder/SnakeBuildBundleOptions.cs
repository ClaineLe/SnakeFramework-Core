using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using com.snake.framework.runtime;

namespace com.snake.framework
{
    namespace editor
    {
        public class SnakeBuildBundleOptions
        {
            public BundleBuildParameters mParameters { get; private set; }
            public string mAssetBundleOutputPath { get; private set; }
            public string mExtOutPutPath { get; private set; }
            public string[] mUsingAssets { get; private set; }

            public SnakeBuildBundleOptions(string assetBundleOutputPath, BuildTarget buildTarget)
            {
                BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                mParameters = new BundleBuildParameters(buildTarget, buildTargetGroup, assetBundleOutputPath);
                this.mAssetBundleOutputPath = assetBundleOutputPath;
            }

            public void SetExtBundleNamePath(string extOutPutPath)
            {
                this.mExtOutPutPath = extOutPutPath;
                this.mUsingAssets = readUsingAssetPathFile();
            }

            private string[] readUsingAssetPathFile() 
            {
                BuilderSetting setting = BuilderSetting.EditorGet();

                if (string.IsNullOrEmpty(setting.mUsingAssetsFilePath))
                {
                    SnakeDebuger.Error("没有找到拓张Bundle列表文件. path:" + setting.mUsingAssetsFilePath);
                    return null;
                }
                List<string> assetPathList = new List<string>();
                string[] assetPaths = System.IO.File.ReadAllLines(setting.mUsingAssetsFilePath);
                foreach (var assetPath in assetPaths)
                {
                    if (string.IsNullOrEmpty(assetPath) == true)
                        continue;
                    assetPathList.Add(assetPath);
                }
                return assetPathList.ToArray();
            }
        }
    }
}