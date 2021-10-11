using com.snake.framework.runtime;

namespace com.snake.framework
{
    /// <summary>
    /// ‘§º”‘ÿ¡˜≥Ã
    /// </summary>
    internal class PreloadProcedure : BaseProcedure
    {
        protected override void onEnter(ProcedureManager owner, IState<ProcedureManager> fromState, object userData)
        {
            base.onEnter(owner, fromState, userData);
            owner.mAppFacade.PreloadManagers();
        }

        protected override void onTick(ProcedureManager owner, int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
        {
            if (owner.mAppFacade.GetProloadProgress() < 1.0f)
                return;
            Singleton<AppFacade>.GetInstance().EnterGameContent();
        }
    }
}
