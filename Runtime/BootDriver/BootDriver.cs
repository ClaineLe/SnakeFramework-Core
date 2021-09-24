namespace com.snake.framework
{
    namespace runtime
    {
        public class BootDriver
        {
            public const string RUNTIME_ASSEMBLY = "Assembly-CSharp";
            public const string CUSTOM_NAMESPACE = "com.snake.framework.custom.runtime";
            public string CustomAppFacadeClassName = "AppFacadeCostom";
            public IAppFacadeCostom mAppFacadeCostom { get; private set; }

            [UnityEngine.RuntimeInitializeOnLoadMethod]
            static public void BootUp()
            {
                BootDriver bootDriver = new BootDriver();
                Singleton<AppFacade>.GetInstance().StartUp(bootDriver.mAppFacadeCostom);
            }

            private BootDriver()
            {
                this.mAppFacadeCostom = _CreateCostomAppFacade();
            }


            private IAppFacadeCostom _CreateCostomAppFacade()
            {
                System.Type type = default;
                System.Reflection.Assembly[] s_Assemblies = Utility.Assembly.GetAssemblies();
                foreach (System.Reflection.Assembly assembly in s_Assemblies)
                {
                    if (assembly.GetName().Name.Equals(RUNTIME_ASSEMBLY))
                    {
                        type = assembly.GetType(CUSTOM_NAMESPACE + "." + CustomAppFacadeClassName);
                        break;
                    }
                }

                if (type == null)
                    throw new System.Exception("没有找到应用门户的自定义实现类型(IAppFacadeCostom)。");

                object appFacadeCostomObj = System.Activator.CreateInstance(type);
                if (appFacadeCostomObj == null)
                    throw new System.Exception("没有找到应用门户的自定义实现对象(IAppFacadeCostom)。");
                return appFacadeCostomObj as IAppFacadeCostom;
            }
        }
    }
}