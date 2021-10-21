namespace com.snake.framework
{
    namespace runtime
    {
        public abstract class BaseProcedure : BaseState<ProcedureManager>, IProcedure
        {
            public virtual string mName
            {
                get
                {
                    return this.GetType().Name;
                }
            }
        }
    }
}
