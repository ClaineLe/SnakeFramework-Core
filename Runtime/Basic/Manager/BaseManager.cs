namespace com.snake.framework
{
    namespace runtime
    {
        public abstract class BaseManager : IManager
        {
            private AppFacade _appFacade;
            public AppFacade mAppFacade 
            {
                get 
                {
                    if (_appFacade == null)
                        _appFacade = Singleton<AppFacade>.GetInstance();
                    return _appFacade;
                }
            }
            public virtual string mName => this.GetType().Name;

            public virtual float mInitProgress { get; } = 1.0f;
            public virtual float mPreloadProgress { get; } = 1.0f;

            public virtual void Initialization()
            { 
            }

            public virtual void Preload() 
            { 
            }
        }
    }
}