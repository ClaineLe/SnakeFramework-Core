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
                get
                {
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
            public SnakeEnvironment mEnvironment { get; private set; }
            public UnityEngine.GameObject mRoot { get; private set; }

            public ISplashUserInterface mSplashUserInterface { get; private set; }
            public IUpdateController mUpdateController { get; private set; }

            protected void initialize()
            {
                mRoot = new UnityEngine.GameObject("SnakeRoot");
                UnityEngine.GameObject.DontDestroyOnLoad(mRoot);
                this.mLifeCycle = LifeCycle.Create(mRoot);
                this._managerDic = new Dictionary<System.Type, IManager>();

                this.mEnvironment = UnityEngine.Resources.Load<SnakeEnvironment>(typeof(SnakeEnvironment).Name);
                if (mEnvironment == null)
                {
                    SnakeDebuger.ErrorFormat("没有在Resources目录下找到SnakeEnvironment.asset，请右键创建一个");
                    return;
                }
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

            public void SetupSplashUserInterface<T>() where T : ISplashUserInterface, new()
            {
                this.mSplashUserInterface = new T();
                this.mSplashUserInterface.Init();
            }

            public void SetupUpdateController<T>() where T : IUpdateController, new() 
            {
                this.mUpdateController = new T();
                this.mUpdateController.Init();
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
                IManager mgr = GetManager(typeof(T));
                if (mgr == null)
                    return null;
                return mgr as T;
            }

            public IManager GetManager(System.Type mgrType)
            {
                if (this._managerDic.TryGetValue(mgrType, out IManager manager) == false)
                    return null;
                return manager;
            }

            internal void InitManagers()
            {
                SnakeDebuger.Log("InitManagers");
                Dictionary<System.Type, IManager>.Enumerator enumerator = this._managerDic.GetEnumerator();
                while (enumerator.MoveNext())
                    enumerator.Current.Value.Initialization();
            }
            internal void PreloadManagers()
            {
                Dictionary<System.Type, IManager>.Enumerator enumerator = this._managerDic.GetEnumerator();
                while (enumerator.MoveNext())
                    enumerator.Current.Value.Preload();
            }

            internal float GetInitProgress()
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

            internal float GetProloadProgress()
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
