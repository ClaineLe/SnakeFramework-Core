namespace com.snake.framework
{
    namespace runtime
    {
        public sealed class BundleOperationCollection : IReference
        {
            public BundleAsyncOperation mMainBundleAsyncOperation;
            public BundleAsyncOperation[] mDependAsyncOperation;

            static public BundleOperationCollection New()
            {
                return ReferencePool.Take<BundleOperationCollection>();
            }

            static public void Release(BundleOperationCollection bundleOperationCollection)
            {
                ReferencePool.Return(bundleOperationCollection);
            }

            public void OnReferenceClear()
            {
                if (mDependAsyncOperation != null && mDependAsyncOperation.Length > 0)
                {
                    foreach (var a in mDependAsyncOperation)
                    {
                        BundleAsyncOperation.Release(a);
                    }
                }
                mDependAsyncOperation = null;
                BundleAsyncOperation.Release(mMainBundleAsyncOperation);
                mMainBundleAsyncOperation = null;
            }
        }
    }
}