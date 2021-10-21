namespace com.snake.framework
{
    namespace runtime
    {
        public abstract class BaseManager : IManager
        {
            public SnakeFramework mFramework => SnakeFramework.Instance;
            public virtual string mName => this.GetType().Name;

            public virtual float GetInitProgress() { return 1.0f; }
            public virtual float GetPreloadProgress() { return 1.0f; }
            protected virtual void onRegiested() { }
            protected virtual void onInitialization() { }
            protected virtual void onPreload() { }

            public void Regiested() { this.onRegiested(); }
            public void Initialization() { this.onInitialization(); }
            public void Preload() { this.onPreload(); }
        }
    }
}