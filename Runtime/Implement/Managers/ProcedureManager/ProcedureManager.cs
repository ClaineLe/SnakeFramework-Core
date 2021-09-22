namespace com.snake.framework
{
    namespace runtime
    {
        public class ProcedureManager : BaseManager, IFiniteStateMachineOwner
        {
            private FiniteStateMachine<ProcedureManager> _procedureFsm;

            public ProcedureManager()
            {
                this._procedureFsm = new FiniteStateMachine<ProcedureManager>(this);
                LifeCycle.mUpdateHandle.AddEventHandler(this._procedureFsm.Tick);
                RegiestProcedure<BootUpProcedure>();
                RegiestProcedure<SplashProcedure>();
                RegiestProcedure<PreloadProcedure>();
            }

            public T RegiestProcedure<T>(bool replace = false)
                where T : BaseProcedure, new()
            {
                T procedure = new T();
                this._procedureFsm.AddState(procedure.GetType().Name, procedure, replace);
                procedure.Init(this);
                return procedure;
            }

            public bool SwitchProcedure<T>(object userData = null, bool autoRegiest = true)
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
