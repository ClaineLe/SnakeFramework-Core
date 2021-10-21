using System;

namespace com.snake.framework
{
    namespace runtime
    {
        public class SnakeEvent
        {
            protected Delegate _delegate;
            public void AddEventHandler(Action cb)
            {
                _delegate = Delegate.Combine(_delegate, cb);
            }
            public void RemoveEventHandler(Action cb)
            {
                _delegate = Delegate.RemoveAll(_delegate, cb);
            }
            public void BroadCastEvent()
            {
                if (_delegate != null)
                {
                    (_delegate as Action)();
                }
            }
        }
        public class SnakeEvent<T0>
        {
            protected Delegate _delegate;
            public void AddEventHandler(Action<T0> cb)
            {
                _delegate = Delegate.Combine(_delegate, cb);
            }
            public void RemoveEventHandler(Action<T0> cb)
            {
                _delegate = Delegate.RemoveAll(_delegate, cb);
            }
            public void BroadCastEvent(T0 arg0)
            {
                if (_delegate != null)
                {
                    (_delegate as Action<T0>)(arg0);
                }
            }
        }
        public class SnakeEvent<T0, T1>
        {
            protected Delegate _delegate;
            public void AddEventHandler(Action<T0, T1> cb)
            {
                _delegate = Delegate.Combine(_delegate, cb);
            }
            public void RemoveEventHandler(Action<T0, T1> cb)
            {
                _delegate = Delegate.RemoveAll(_delegate, cb);
            }
            public void BroadCastEvent(T0 arg0, T1 arg1)
            {
                if (_delegate != null)
                {
                    (_delegate as Action<T0, T1>)(arg0, arg1);
                }
            }
        }
        public class SnakeEvent<T0, T1, T2>
        {
            protected Delegate _delegate;
            public void AddEventHandler(Action<T0, T1, T2> cb)
            {
                _delegate = Delegate.Combine(_delegate, cb);
            }
            public void RemoveEventHandler(Action<T0, T1, T2> cb)
            {
                _delegate = Delegate.RemoveAll(_delegate, cb);
            }
            public void BroadCastEvent(T0 arg0, T1 arg1, T2 arg2)
            {
                if (_delegate != null)
                {
                    (_delegate as Action<T0, T1, T2>)(arg0, arg1, arg2);
                }
            }
        }

        public class SnakeEvent<T0, T1, T2, T3>
        {
            protected Delegate _delegate;
            public void AddEventHandler(Action<T0, T1, T2, T3> cb)
            {
                _delegate = Delegate.Combine(_delegate, cb);
            }
            public void RemoveEventHandler(Action<T0, T1, T2, T3> cb)
            {
                _delegate = Delegate.RemoveAll(_delegate, cb);
            }
            public void BroadCastEvent(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
            {
                if (_delegate != null)
                {
                    (_delegate as Action<T0, T1, T2, T3>)(arg0, arg1, arg2, arg3);
                }
            }
        }
        public class SnakeEvent<T0, T1, T2, T3, T4>
        {
            protected Delegate _delegate;
            public void AddEventHandler(Action<T0, T1, T2, T3, T4> cb)
            {
                _delegate = Delegate.Combine(_delegate, cb);
            }
            public void RemoveEventHandler(Action<T0, T1, T2, T3, T4> cb)
            {
                _delegate = Delegate.RemoveAll(_delegate, cb);
            }
            public void BroadCastEvent(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            {
                if (_delegate != null)
                {
                    (_delegate as Action<T0, T1, T2, T3, T4>)(arg0, arg1, arg2, arg3, arg4);
                }
            }
        }
    }
}
