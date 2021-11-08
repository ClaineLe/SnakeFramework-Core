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

            [Header("构建配置路径")]
            public string mBuilderSettingPath;
        }
    }
}