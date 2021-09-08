using com.snake.framework.runtime;
using System.Collections.Generic;

namespace com.snake.framework
{
    namespace runtime
    {
        public class AppFacade : Singleton<AppFacade>, ISingleton
        {
            public LifeCycle mLifeCycle;
            private Dictionary<string, IManager> _managerDic;

            protected override void onInitialize()
            {
                base.onInitialize();
                this._managerDic = new Dictionary<string, IManager>();
                this.mLifeCycle = LifeCycle.Create();
            }

            public void StartUp(BootDriver bootDriver)
            {
                this._RegiestManager();
                bootDriver.mAppFacadeCostom.Initialization();
                bootDriver.mAppFacadeCostom.GameLaunch();
            }

            public T RegiestManager<T>() where T : IManager, new()
            {
                T manager = new T();
                if (_managerDic.ContainsKey(manager.mName) == true)
                {
                    throw new System.Exception("管理器已经存在.MgrName:" + manager.mName);
                }

                manager.Initialization();
                _managerDic.Add(manager.mName, manager);
                return manager;
            }

            public T GetManager<T>()
                where T : class, IManager
            {
                return GetManager(typeof(T).Name) as T;
            }

            public IManager GetManager(string mgrName)
            {
                IManager manager;
                if (this._managerDic.TryGetValue(mgrName, out manager))
                {
                    return manager;
                }
                return null;
            }


            private void _RegiestManager()
            {
                this.RegiestManager<NetworkManager>();
                this.RegiestManager<ProcedureManager>();
                this.RegiestManager<UIManager>();
            }

        }
    }
}
