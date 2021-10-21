namespace com.snake.framework
{
    namespace runtime
    {
        public interface IUpdateController
        {
            bool mIsDone { get; }
            void Enter();
            void Tick();
            void Dispose();
        }
    }
}
