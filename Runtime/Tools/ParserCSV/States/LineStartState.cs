namespace com.snake.framework
{
    namespace runtime
    {
        public class LineStartState : ParserState
        {
            public LineStartState(ParserStateMachine machine) : base(machine)
            {

            }

            public override void AnyChar(char ch)
            {
                machine.context.AddChar(ch);
                machine.SetState(machine.ValueState);
            }

            public override void Comma()
            {
                machine.context.AddValue();
                machine.SetState(machine.ValueStartState);
            }

            public override void EndOfLine()
            {
                machine.context.AddLine();
                machine.SetState(machine.LineStartState);
            }

            public override void Quote()
            {
                machine.SetState(machine.QuotedValueState);
            }
        }
    }
}