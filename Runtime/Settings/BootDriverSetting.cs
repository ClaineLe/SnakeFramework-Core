using UnityEngine;

namespace com.snake.framework
{
    namespace runtime
    {

        [UnityEngine.CreateAssetMenu(fileName = "SnakeScriptableObject/BootDriverSetting")]
        public class BootDriverSetting : SnakeBasicSetting<BootDriverSetting>
        {
            [Header("启动场景的Tag启动标签")]
            public string BootUpTagName = "BootUp";
            
            [Header("框架拓展子类程序集名")]
            public string FrameworkExtTypeAssemblyName = "Assembly-CSharp";
            
            [Header("框架拓展子类全名")]
            public string FrameworkExtTypeFullName = "com.snake.framework.custom.runtime.FrameworkExt";

        }
    }
}