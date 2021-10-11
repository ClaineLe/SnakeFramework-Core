using UnityEngine;

namespace com.snake.framework
{
    namespace runtime
    {
        public class LifeCycle : MonoBehaviour
        {
            static public SnakeEvent mStartHandle = new SnakeEvent();
            static public SnakeEvent mApplicationQuitHandle = new SnakeEvent();
            static public SnakeEvent<bool> mApplicationFocusHandle = new SnakeEvent<bool>();
            static public SnakeEvent<bool> mApplicationPauseHandle = new SnakeEvent<bool>();
            static public SnakeEvent<int, float, float, float, float> mFixedUpdateHandle = new framework.SnakeEvent<int, float, float, float, float>();
            static public SnakeEvent<int, float, float, float, float> mUpdateHandle = new framework.SnakeEvent<int, float, float, float, float>();
            static public SnakeEvent<int, float, float, float, float> mLateUpdateHandle = new framework.SnakeEvent<int, float, float, float, float>();

            static public void Initialization()
            {
                GameObject lifeCycleRoot = new UnityEngine.GameObject("LifeCycle", new[] { typeof(LifeCycle) });
                GameObject.DontDestroyOnLoad(lifeCycleRoot);
            }

            private void Start()
            {
                mStartHandle?.BroadCastEvent();
            }

            private void FixedUpdate()
            {
                mFixedUpdateHandle?.BroadCastEvent(Time.frameCount, Time.time, Time.deltaTime, Time.unscaledTime, Time.realtimeSinceStartup);
            }

            private void Update()
            {
                mUpdateHandle?.BroadCastEvent(Time.frameCount, Time.time, Time.deltaTime, Time.unscaledTime, Time.realtimeSinceStartup);
            }

            private void LateUpdate()
            {
                mLateUpdateHandle?.BroadCastEvent(Time.frameCount, Time.time, Time.deltaTime, Time.unscaledTime, Time.realtimeSinceStartup);
            }

            private void OnApplicationFocus(bool focus)
            {
                mApplicationFocusHandle?.BroadCastEvent(focus);
            }

            private void OnApplicationPause(bool pause)
            {
                mApplicationPauseHandle?.BroadCastEvent(pause);

            }

            private void OnApplicationQuit()
            {
                mApplicationQuitHandle?.BroadCastEvent();
            }
        }
    }
}
