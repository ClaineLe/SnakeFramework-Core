namespace com.snake.framework
{
    namespace runtime
    {
        internal class UpdateProcedure : BaseProcedure, IFiniteStateMachineOwner
        {
            private IUpdateController _updateController;

            protected override void onEnter(ProcedureManager owner, IState<ProcedureManager> fromState, object userData)
            {
                base.onEnter(owner, fromState, userData);
                if (owner.mFramework.mUpdateController == null)
                {
                    throw new System.Exception("没有实现热更控制器");
                }
                this._updateController = owner.mFramework.mUpdateController;
                this._updateController?.Enter();
            }

            protected override void onTick(ProcedureManager owner, int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
            {
                this._updateController?.Tick(frameCount, time, deltaTime, unscaledTime, realElapseSeconds);
                if (this._updateController?.mIsDone == false)
                    return;
                owner.SwitchProcedure<PreloadProcedure>();
            }

            protected override void onExit(ProcedureManager owner, IState<ProcedureManager> toState)
            {
                base.onExit(owner, toState);
                if (this._updateController != null)
                {
                    this._updateController.Dispose();
                    this._updateController = null;
                }
            }
        }
    }
}
