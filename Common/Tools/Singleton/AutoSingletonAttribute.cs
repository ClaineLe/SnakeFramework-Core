namespace com.snake.framework
{
    namespace runtime
    {
        public class AutoSingletonAttribute : System.Attribute
        {
            public bool bAutoCreate;

            public AutoSingletonAttribute(bool bCreate)
            {
                this.bAutoCreate = bCreate;
            }
        }
    }
}
