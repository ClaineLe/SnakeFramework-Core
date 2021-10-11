using System.Collections.Generic;

namespace com.snake.framework
{
    namespace runtime
    {
        public class AppFacade : Singleton<AppFacade>, ISingleton
        {
            public LifeCycle mLifeCycle;
            private Dictionary<System.Type, IManager> _managerDic;
            private IAppFacadeCostom _appFacadeCostom;
            protected override void onInitialize()
            {
                base.onInitialize();
                LifeCycle.Initialization();
                this._managerDic = new Dictionary<System.Type, IManager>();
            }

            private void _regiestBuildInManager() 
            {
                RegiestManager<ProcedureManager>();
                RegiestManager<DownloadManager>();
            }

            public void StartUp(IAppFacadeCostom appFacadeCostom)
            {
                this._appFacadeCostom = appFacadeCostom;
                this._regiestBuildInManager();
                this._appFacadeCostom.Initialization();
                this.GetManager<ProcedureManager>()?.SwitchProcedure<BootUpProcedure>();
            }

            public void EnterGameContent()
            {
                this._appFacadeCostom.EnterGameContent();
            }

            public T GetAppFacadeCostom<T>()
                where T : class, IAppFacadeCostom
            {
                return _appFacadeCostom as T;
            }

            public T RegiestManager<T>(bool replace = false) where T : IManager
            {
                System.Type mgrType = typeof(T);
                if (replace == false && _managerDic.ContainsKey(mgrType) == true)
                {
                    throw new System.Exception("管理器已经存在.MgrName:" + mgrType);
                }
                T manager = (T)System.Activator.CreateInstance(mgrType);
                _managerDic[mgrType] = manager;
                return manager;
            }

            public T GetManager<T>() where T : class, IManager
            {
                if (this._managerDic.TryGetValue(typeof(T), out IManager manager) == false)
                    return null;
                return manager as T;
            }

            public void InitManagers()
            {
                Dictionary<System.Type, IManager>.Enumerator enumerator = this._managerDic.GetEnumerator();
                while (enumerator.MoveNext())
                    enumerator.Current.Value.Initialization();
            }

            public void PreloadManagers()
            {
                Dictionary<System.Type, IManager>.Enumerator enumerator = this._managerDic.GetEnumerator();
                while (enumerator.MoveNext())
                    enumerator.Current.Value.Preload();
            }

            public float GetInitProgress()
            {
                float count = this._managerDic.Count;
                float progress = 0.0f;
                var enumerator = this._managerDic.GetEnumerator();
                while (enumerator.MoveNext())
                    progress += enumerator.Current.Value.GetInitProgress();
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
                    progress += enumerator.Current.Value.GetPreloadProgress();
                if (count == progress)
                    return 1.0f;
                return progress / count;
            }
        }
    }
}
