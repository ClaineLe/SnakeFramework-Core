using UnityEngine;
namespace com.snake.framework
{
    namespace runtime
    {
        public abstract class ParserState
        {
            protected ParserStateMachine machine;
            protected ParserState(ParserStateMachine machine)
            {
                this.machine = machine;
            }

            public virtual void AnyChar(char ch) { Debug.LogError("Wrong"); }
            public virtual void Comma() { Debug.LogError("Wrong"); }
            public virtual void Quote() { Debug.LogError("Wrong"); }
            public virtual void EndOfLine() { Debug.LogError("Wrong"); }
        }
    }
}
