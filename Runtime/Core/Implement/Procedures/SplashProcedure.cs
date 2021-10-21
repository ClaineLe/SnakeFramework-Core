namespace com.snake.framework
{
    namespace runtime
    {
        /// <summary>
        /// ÉÁÆÁÁ÷³Ì
        /// </summary>
        internal class SplashProcedure : BaseProcedure
        {
            private ISplashUserInterface _splashUserInterface;
            protected override void onEnter(ProcedureManager owner, IState<ProcedureManager> fromState, object userData)
            {
                base.onEnter(owner, fromState, userData);
             
                SnakeFramework.Instance.InitManagers();

                if (owner.mFramework.mSplashUserInterface != null)
                {
                    this._splashUserInterface = owner.mFramework.mSplashUserInterface;
                    this._splashUserInterface.Start();
                }
            }

            protected override void onTick(ProcedureManager owner, int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
            {
                float progress = SnakeFramework.Instance.GetInitProgress();
                this._splashUserInterface?.Tick(progress);

                if (this._splashUserInterface != null && this._splashUserInterface.mIsDone == false)
                    return;

                if (progress < 1.0f)
                    return;

                if (SnakeFramework.Instance.mUpdateController != null)
                {
                    owner.SwitchProcedure<UpdateProcedure>();
                    return;
                }
                owner.SwitchProcedure<PreloadProcedure>();
            }
        }
    }
}
