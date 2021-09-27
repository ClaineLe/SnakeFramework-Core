namespace com.snake.framework
{
    namespace runtime
    {
        [UnityEngine.CreateAssetMenu()]
        public class BootDriverSetting : UnityEngine.ScriptableObject
        {
            public bool Active = true;
            public string BootUpTagName = "BootUp";
            public string RuntimeAssemblyName = "Assembly-CSharp";
            public string CustomAppFacadeTypeFullName = "com.snake.framework.custom.runtime.AppFacadeCostom";
        }
    }
}