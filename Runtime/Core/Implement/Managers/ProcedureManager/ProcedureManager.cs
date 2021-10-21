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
                mFramework.mLifeCycle.mUpdateHandle.AddEventHandler(this._procedureFsm.Tick);
            }

            public T RegiestProcedure<T>()
                where T : BaseProcedure, new()
            {
                BaseProcedure procedure = RegiestProcedure(typeof(T));
                if (procedure == null)
                    return null;
                return procedure as T;
            }
            
            public BaseProcedure RegiestProcedure(System.Type procedureType)
            {
                BaseProcedure procedure = System.Activator.CreateInstance(procedureType) as BaseProcedure;
                this._procedureFsm.AddState(procedure.GetType().Name, procedure, false);
                procedure.Init(this);
                return procedure;
            }
            
            public bool SwitchProcedure<T>(object userData = null, bool autoRegiest = true)
                where T : BaseProcedure, new()
            {
                return SwitchProcedure(typeof(T), userData, autoRegiest);
            }
            
            public bool SwitchProcedure(System.Type procedureType, object userData = null, bool autoRegiest = true)
            {
                string procedureName = procedureType.Name;
                SnakeDebuger.Log("SwitchProcedure:" + procedureName);
                if (this._procedureFsm.HasState(procedureName) == false)
                {
                    if (autoRegiest == false)
                    {
                        SnakeDebuger.ErrorFormat("切换流程失败。不存在流程:{0}", procedureName);
                        return false;
                    }
                    this.RegiestProcedure(procedureType);
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
