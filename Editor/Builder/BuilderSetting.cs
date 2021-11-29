using com.snake.framework.runtime;
using UnityEngine;

namespace com.snake.framework
{
    namespace editor
    {
        [UnityEngine.CreateAssetMenu(fileName = "BuilderSetting")]
        public class BuilderSetting : SnakeBasicSetting<BuilderSetting>
        {
            [Header("[Editor]资源规则目录")]
            public string mAssetRulesPath;

            [Header("[Editor]Bundle缓存目录")]
            public string mBundleCachePath;

            [Header("[Editor]被用资源录像文件")]
            public string mDefAssetRecordFile;

            static public BuilderSetting EditorGet()
            {
                EnvironmentSetting envSetting = EnvironmentSetting.Get();
                if (envSetting == null)
                    return null;
                return editorGet(envSetting.mBuilderSettingPath);
            }

        }
    }
}
