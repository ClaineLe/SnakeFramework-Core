namespace com.snake.framework
{
    namespace runtime
    {
        public class UnscaleTimer : DurationTimer
        {
            protected override bool tickProcess(int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
            {
                _currDuration += unscaledTime;
                return _currDuration >= _endDuration;
            }
        }
    }
}