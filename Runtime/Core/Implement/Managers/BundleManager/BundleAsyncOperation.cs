using UnityEngine;

namespace com.snake.framework
{
    namespace runtime
    {
        public sealed class BundleAsyncOperation : IReference
        {
            public string mBundleFullName;
            public AssetBundleCreateRequest mAssetBundleCreateRequest;
            public int mReferenceCount;
            public AssetBundle GetAssetBundle()
            {
                if (this.mAssetBundleCreateRequest == null)
                    return null;
                return this.mAssetBundleCreateRequest.assetBundle;
            }
            public bool GetIsDone()
            {
                if (mAssetBundleCreateRequest == null)
                    return false;
                return mAssetBundleCreateRequest.isDone;
            }
            static public BundleAsyncOperation New(string loadPath, string bundleName)
            {
                BundleAsyncOperation bundleAsyncOperation = ReferencePool.Take<BundleAsyncOperation>();
                bundleAsyncOperation.mAssetBundleCreateRequest = AssetBundle.LoadFromFileAsync(loadPath);
                bundleAsyncOperation.mBundleFullName = bundleName;
                return bundleAsyncOperation;
            }

            static public void Release(BundleAsyncOperation bundleAsyncOperation)
            {
                AssetBundle assetBundle = bundleAsyncOperation.GetAssetBundle();
                assetBundle?.Unload(true);
                ReferencePool.Return(bundleAsyncOperation);
            }

            public void AddReferenceCount()
            {
                ++this.mReferenceCount;
            }

            public void ReduceReferenceCount()
            {
                --this.mReferenceCount;
            }

            public void OnReferenceClear()
            {
                this.mBundleFullName = string.Empty;
                mAssetBundleCreateRequest = null;
                mReferenceCount = 0;
            }
        }
    }
}