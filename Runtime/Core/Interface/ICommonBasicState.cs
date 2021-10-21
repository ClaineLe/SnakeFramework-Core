namespace com.snake.framework
{
    namespace runtime
    {
        public interface ICommonBasicState
        {
            /// <summary>
            /// 名字
            /// </summary>
            string Name { get; }

            /// <summary>
            /// 是否完成
            /// </summary>
            bool mIsDone { get; }

            /// <summary>
            /// 进度
            /// </summary>
            float mProgress { get; }

            /// <summary>
            /// 报错信息
            /// </summary>
            string mError { get; }

            /// <summary>
            /// 初始化
            /// </summary>
            void Init();

            /// <summary>
            /// 进入状态
            /// </summary>
            /// <param name="userData"></param>
            void Enter(object userData = null);

            /// <summary>
            /// 每帧更新
            /// </summary>
            /// <param name="frameCount"></param>
            /// <param name="time"></param>
            /// <param name="deltaTime"></param>
            /// <param name="unscaledTime"></param>
            /// <param name="realElapseSeconds"></param>
            void Tick(int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds);

            /// <summary>
            /// 退出状态
            /// </summary>
            void Exit();

            /// <summary>
            /// 销毁状态
            /// </summary>
            void Dispose();
        }
    }
}
