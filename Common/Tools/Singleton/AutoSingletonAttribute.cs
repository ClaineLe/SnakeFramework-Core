namespace com.snake.framework
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
