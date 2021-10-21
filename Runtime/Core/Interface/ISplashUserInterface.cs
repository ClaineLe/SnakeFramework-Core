namespace com.snake.framework
{
    namespace runtime
    {
        public interface ISplashUserInterface
        {
            bool mIsDone { get; }
            void Start();
            void Tick(float progress);
        }
    }
}
