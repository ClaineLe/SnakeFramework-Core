namespace com.snake.framework
{
    namespace runtime
    {
        public interface IManager
        {
            string mName { get; }
            float mInitProgress { get; }
            float mPreloadProgress { get; }
            void Initialization();

            void Preload();
        }
    }
}