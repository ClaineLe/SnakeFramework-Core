using System.Collections.Generic;

namespace com.snake.framework
{
    namespace runtime
    {
        public class AppFacade : Singleton<AppFacade>, ISingleton
        {
            public LifeCycle mLifeCycle;
            private Dictionary<System.Type, IManager> _managerDic;

            protected override void onInitialize()
            {
                base.onInitialize();
                this._managerDic = new Dictionary<System.Type, IManager>();
                this.mLifeCycle = LifeCycle.Create();
            }

            public void StartUp(BootDriver bootDriver)
            {
                this._RegiestManager();
                bootDriver.mAppFacadeCostom.Initialization();
                bootDriver.mAppFacadeCostom.GameLaunch();
            }

            public void RegiestManager<T>(bool replace = false) where T : IManager
            {
                System.Type mgrType = typeof(T);
                if (replace == false && _managerDic.ContainsKey(mgrType) == true)
                {
                    throw new System.Exception("管理器已经存在.MgrName:" + mgrType);
                }
                _managerDic[mgrType] = System.Activator.CreateInstance(mgrType) as IManager;
            }

            public T GetManager<T>() where T : class, IManager
            {
                if (this._managerDic.TryGetValue(typeof(T), out IManager manager) == false)
                    return null;
                return manager as T;
            }

            private void _RegiestManager()
            {
                this.RegiestManager<AssetManager>();
                this.RegiestManager<ProcedureManager>();
                this.RegiestManager<NetworkManager>();
                this.RegiestManager<UIManager>();
            }

            public void InitManagers()
            {
                foreach (var a in _managerDic)
                {
                    a.Value.Initialization();
                }
            }

            public float GetInitProgress()
            {
                float count = this._managerDic.Count;
                float progress = 0.0f;
                var enumerator = this._managerDic.GetEnumerator();
                while (enumerator.MoveNext())
                    progress += enumerator.Current.Value.mInitProgress;
                if (count == progress)
                    return 1.0f;
                return progress / count;
            }

            public float GetProloadProgress()
            {
                float count = this._managerDic.Count;
                float progress = 0.0f;
                var enumerator = this._managerDic.GetEnumerator();
                while (enumerator.MoveNext())
                    progress += enumerator.Current.Value.mPreloadProgress ;
                if (count == progress)
                    return 1.0f;
                return progress / count;
            }
        }
    }
}
