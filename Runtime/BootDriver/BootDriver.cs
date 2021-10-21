using UnityEngine.SceneManagement;

namespace com.snake.framework
{
    namespace runtime
    {
        internal class BootDriver
        {
            public ISnakeFrameworkExt mSnakeFrameworkExt { get; private set; }

            [UnityEngine.RuntimeInitializeOnLoadMethod]
            static public void BootUp()
            {
                BootDriverSetting bootDriverSetting = UnityEngine.Resources.Load<BootDriverSetting>(typeof(BootDriverSetting).Name);
                if (bootDriverSetting == null)
                {
                    SnakeDebuger.ErrorFormat("没有在Resources目录下找到BootDriverSetting.asset，请右键创建一个");
                    return;
                }

                if (bootDriverSetting.Active == false)
                {
                    SnakeDebuger.ErrorFormat("框架未激活。bootDriverSetting.Active == false");
                    return;
                }

                if (string.IsNullOrEmpty(bootDriverSetting.BootUpTagName) == true)
                {
                    SnakeDebuger.ErrorFormat("启动标记Tag不能为空。bootDriverSetting.BootUpTagName：" + bootDriverSetting.BootUpTagName);
                    return;
                }

                try
                {
                    if (UnityEngine.GameObject.FindWithTag(bootDriverSetting.BootUpTagName) == null)
                    {
                        SnakeDebuger.ErrorFormat("没有找到Tag标记为：{0} 的GameObject对象，不是游戏启动场景", bootDriverSetting.BootUpTagName);
                        return;
                    }
                }
                catch (UnityEngine.UnityException unityEx)
                {
                    SnakeDebuger.Error("没有找到启动Tag:" + bootDriverSetting.BootUpTagName + ".(" + unityEx.Message + ")");
                    return;
                }


                BootDriver bootDriver = new BootDriver(bootDriverSetting);
                SnakeFramework.Instance.StartUp(bootDriver.mSnakeFrameworkExt);
            }
            private BootDriver(BootDriverSetting bootDriverSetting)
            {
                System.Type type = default;
                System.Reflection.Assembly[] s_Assemblies = Utility.Assembly.GetAssemblies();
                foreach (System.Reflection.Assembly assembly in s_Assemblies)
                {
                    if (assembly.GetName().Name.Equals(bootDriverSetting.RuntimeAssemblyName))
                    {
                        type = assembly.GetType(bootDriverSetting.FrameworkExtTypeFullName);
                        break;
                    }
                }

                if (type == null)
                    throw new System.Exception("没有找到应用门户的自定义实现类型(IAppFacadeCostom)。");

                object appFacadeCostomObj = System.Activator.CreateInstance(type);
                if (appFacadeCostomObj == null)
                    throw new System.Exception("没有找到应用门户的自定义实现对象(IAppFacadeCostom)。");
                this.mSnakeFrameworkExt = appFacadeCostomObj as ISnakeFrameworkExt;
            }
        }
    }
}