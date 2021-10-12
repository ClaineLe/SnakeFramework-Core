namespace com.snake.framework
{
    namespace runtime
    {
        [UnityEngine.CreateAssetMenu(fileName = "BootDriverSetting")]
        public class BootDriverSetting : UnityEngine.ScriptableObject
        {
            public bool Active = true;
            public string BootUpTagName = "BootUp";
            public string RuntimeAssemblyName = "Assembly-CSharp";
            public string FrameworkExtTypeFullName = "com.snake.framework.custom.runtime.FrameworkExt";
        }
    }
}