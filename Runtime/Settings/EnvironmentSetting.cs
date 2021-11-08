using UnityEngine;

namespace com.snake.framework
{
    namespace runtime
    {
        [UnityEngine.CreateAssetMenu(fileName = "EnvironmentSetting")]
        public class EnvironmentSetting : SnakeBasicSetting<EnvironmentSetting>
        {
            [Header("资源根目录路径")]
            public string mResRootPath;

#if UNITY_EDITOR
            [Header("[Editor]构建配置路径(BuilderSetting.asset)")]
            public string mBuilderSettingPath;
#endif
        }
    }
}