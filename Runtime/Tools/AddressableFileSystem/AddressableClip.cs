namespace com.snake.framework
{
    namespace runtime
    {
        public sealed class AddressableClip
        {
            public string address;
            public long length;
            public long offset;
            public bool obsolete;
            public bool isRead;
            public byte[] content;
        }
    }
}
