using UnityEngine;

namespace com.snake.framework
{
    namespace runtime
    {
        public class LifeCycle : MonoBehaviour
        {
            public SnakeEvent mStartHandle = new SnakeEvent();

            public SnakeEvent mGUIHandle = new SnakeEvent();
            public SnakeEvent<int, float, float, float, float> mFixedUpdateHandle = new SnakeEvent<int, float, float, float, float>();
            public SnakeEvent<int, float, float, float, float> mUpdateHandle = new SnakeEvent<int, float, float, float, float>();
            public SnakeEvent<int, float, float, float, float> mLateUpdateHandle = new SnakeEvent<int, float, float, float, float>();

            public SnakeEvent mApplicationQuitHandle = new SnakeEvent();
            public SnakeEvent<bool> mApplicationFocusHandle = new SnakeEvent<bool>();
            public SnakeEvent<bool> mApplicationPauseHandle = new SnakeEvent<bool>();

            static public LifeCycle Create(GameObject frameworkRoot)
            {
                return frameworkRoot.AddComponent<LifeCycle>();
            }

            private void Start()
            {
                mStartHandle?.BroadCastEvent();
            }
            private void OnGUI()
            {
                mGUIHandle?.BroadCastEvent();
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
