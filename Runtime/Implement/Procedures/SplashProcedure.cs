using com.snake.framework.runtime;

namespace com.snake.framework
{
    /// <summary>
    /// ÉÁÆÁÁ÷³Ì
    /// </summary>
    internal class SplashProcedure : BaseProcedure
    {
        protected override void onEnter(ProcedureManager owner, IState<ProcedureManager> fromState, object userData)
        {
            base.onEnter(owner, fromState, userData);
            owner.mAppFacade.InitManagers();
        }

        protected override void onTick(ProcedureManager owner, int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
        {
            if (owner.mAppFacade.GetInitProgress() < 1.0f)
                return;
            owner.SwitchProcedure<PreloadProcedure>();
        }
    }
}
