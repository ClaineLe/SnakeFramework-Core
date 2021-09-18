namespace com.snake.framework
{
    namespace runtime
    {
        public class ProcedureManager : BaseManager, IFiniteStateMachineOwner
        {
            private IFiniteStateMachine<ProcedureManager> _procedureFsm;

            public ProcedureManager() 
            {
                this._procedureFsm = new FiniteStateMachine<ProcedureManager>(this);
                LifeCycle.mUpdateHandle.AddEventHandler(this._procedureFsm.Tick);
            }

            public T RegiestProcedure<T>(bool replace = false)
                where T : BaseProcedure, new()
            {
                T procedure = this._procedureFsm.AddState<T>();
                procedure.Init(this);
                return procedure;
            }

            public bool SwitchProcedure<T>(bool autoRegiest = true, object userData = null)
                where T : BaseProcedure, new()
            {
                string procedureName = typeof(T).Name;
                if (this._procedureFsm.HasState(procedureName) == false)
                {
                    if (autoRegiest == false)
                    {
                        Debuger.ErrorFormat("切换流程失败。不存在流程:{0}", procedureName);
                        return false;
                    }
                    this.RegiestProcedure<T>();
                }
                return this._procedureFsm.Switch(procedureName, userData);
            }
            public bool CanSwitch()
            {
                return this._procedureFsm.CanSwitch();
            }

        }
    }
}
