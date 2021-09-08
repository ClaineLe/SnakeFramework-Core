namespace com.snake.framework
{
    namespace runtime
    {
        public abstract class BaseManager : IManager
        {
            private AppFacade _appFacade;
            protected AppFacade mAppFacade 
            {
                get 
                {
                    if (_appFacade == null)
                        _appFacade = Singleton<AppFacade>.GetInstance();
                    return _appFacade;
                }
            }
            public virtual string mName => this.GetType().Name;

            async public virtual void Initialization() { }
        }
    }
}