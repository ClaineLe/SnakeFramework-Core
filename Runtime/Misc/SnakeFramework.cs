using System.Collections.Generic;

namespace com.snake.framework
{
    namespace runtime
    {
        public class SnakeFramework
        {
            static private SnakeFramework _instance;
            static public SnakeFramework Instance
            {
                get {
                    if (_instance == null)
                    { 
                        _instance = new SnakeFramework();
                        _instance.initialize();
                    }
                    return _instance;
                }
            }

            private Dictionary<System.Type, IManager> _managerDic;
            private ISnakeFrameworkExt _snakeFrameworkExt;
            public LifeCycle mLifeCycle { get; private set; }
            public UnityEngine.GameObject mRoot { get; private set; }
            protected void initialize()
            {
                mRoot = new UnityEngine.GameObject("SnakeRoot");
                UnityEngine.GameObject.DontDestroyOnLoad(mRoot);
                this.mLifeCycle = LifeCycle.Create(mRoot);
                this._managerDic = new Dictionary<System.Type, IManager>();
            }

            public void StartUp(ISnakeFrameworkExt snakeFrameworkExt)
            {
                this._snakeFrameworkExt = snakeFrameworkExt;
                RegiestManager<TimerManager>();
                RegiestManager<DownloadManager>();
                RegiestManager<ProcedureManager>();
                this._snakeFrameworkExt.Initialization();
                GetManager<ProcedureManager>().SwitchProcedure<BootUpProcedure>();
            }

            public void EnterGameContent()
            {
                this._snakeFrameworkExt.EnterGameContent();
            }

            public T RegiestManager<T>(bool replace = false) where T : IManager
            {
                System.Type mgrType = typeof(T);
                if (replace == false && _managerDic.ContainsKey(mgrType) == true)
                {
                    throw new System.Exception("管理器已经存在.MgrName:" + mgrType);
                }
                T manager = (T)System.Activator.CreateInstance(mgrType);
                manager.Regiested();
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
