namespace com.snake.framework
{
    namespace runtime
    {
        public class FrameTimer : BaseTimer
        {
            private int _endFrameNum;
            private int _currFrameNum;

            public override void OnReferenceClear()
            {
                this._endFrameNum = 0;
                this._currFrameNum = 0;
                base.OnReferenceClear();
            }

            public void SetEndFrameNum(int endFrameNum)
            {
                this._endFrameNum = endFrameNum;
            }

            protected override bool tickProcess(int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
            {
                return ++_currFrameNum >= _endFrameNum;
            }
        }
    }
}