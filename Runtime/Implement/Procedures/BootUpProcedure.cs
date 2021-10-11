using com.snake.framework.runtime;

namespace com.snake.framework
{
    internal class BootUpProcedure : BaseProcedure
    {
        protected override void onEnter(ProcedureManager owner, IState<ProcedureManager> fromState, object userData)
        {
            base.onEnter(owner, fromState, userData);
        }

        protected override void onTick(ProcedureManager owner, int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
        {
            owner.SwitchProcedure<SplashProcedure>();
        }
    }
}
