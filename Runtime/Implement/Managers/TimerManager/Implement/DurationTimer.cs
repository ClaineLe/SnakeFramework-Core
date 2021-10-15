namespace com.snake.framework
{
    namespace runtime
    {
        public class DurationTimer : BaseTimer
        {
            protected float _endDuration;
            protected float _currDuration;

            public override void OnReferenceClear()
            {
                this._currDuration = 0;
                this._endDuration = 0;
                base.OnReferenceClear();
            }

            public void SetEndDuration(float endDuration)
            {
                this._endDuration = endDuration;
            }

            protected override bool tickProcess(int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
            {
                _currDuration += deltaTime;
                return _currDuration >= _endDuration;
            }
        }
    }
}