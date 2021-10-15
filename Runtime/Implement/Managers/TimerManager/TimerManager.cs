using System.Collections.Generic;

namespace com.snake.framework
{
    namespace runtime
    {
        public class TimerManager : BaseManager
        {
            private int _timerIndex = int.MinValue;
            private Dictionary<int, BaseTimer> _timerDic;
            private List<BaseTimer> _completedTimerList = new List<BaseTimer>();

            protected override void onInitialization()
            {
                this._timerDic = new Dictionary<int, BaseTimer>();
                this._completedTimerList = new List<BaseTimer>();
            }
            protected override void onPreload()
            {
                base.onPreload();
                mFramework.mLifeCycle.mUpdateHandle.AddEventHandler(this.onTick);
            }

            public int StartTimer(float durationTime, System.Action onCompletedHandle, bool unscaledTime = false)
            {
                DurationTimer timer = unscaledTime ? ReferencePool.Take<UnscaleTimer>() : ReferencePool.Take<DurationTimer>();
                timer.Init(++_timerIndex, onCompletedHandle,
                   UnityEngine.Time.frameCount,
                   UnityEngine.Time.time,
                   UnityEngine.Time.deltaTime,
                   UnityEngine.Time.unscaledTime,
                   UnityEngine.Time.realtimeSinceStartup);
                timer.SetEndDuration(durationTime);
                _timerDic.Add(timer.mId, timer);
                return timer.mId;
            }

            public int StartFramer(int frameCount, System.Action onCompletedHandle)
            {
                FrameTimer timer = ReferencePool.Take<FrameTimer>();
                timer.Init(++_timerIndex, onCompletedHandle,
                   UnityEngine.Time.frameCount,
                   UnityEngine.Time.time,
                   UnityEngine.Time.deltaTime,
                   UnityEngine.Time.unscaledTime,
                   UnityEngine.Time.realtimeSinceStartup);
                timer.SetEndFrameNum(frameCount);
                _timerDic.Add(timer.mId, timer);
                return timer.mId;
            }

            public bool CancelFramer(int timerId)
            {
                if (_timerDic.TryGetValue(timerId, out BaseTimer baseTimer) == false)
                    return false;
                baseTimer.Cancel();
                return true;
            }

            protected void onTick(int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
            {
                Dictionary<int, BaseTimer>.Enumerator enumerator = _timerDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BaseTimer timer = enumerator.Current.Value;
                    timer.Tick(frameCount, time, deltaTime, unscaledTime, realElapseSeconds);
                    if (timer.mCompleted == true || timer.mCancel)
                        _completedTimerList.Add(timer);
                }

                if (_completedTimerList.Count > 0)
                {
                    for (int i = 0; i < _completedTimerList.Count; i++)
                    {
                        _timerDic.Remove(_completedTimerList[i].mId);
                        ReferencePool.Return(_completedTimerList[i]);
                    }
                    _completedTimerList.Clear();
                }
            }
        }
    }
}