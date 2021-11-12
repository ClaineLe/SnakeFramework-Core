using System.Collections.Generic;
using System.IO;

namespace com.snake.framework
{
    namespace runtime
    {
        public class BundleManager : BaseManager
        {
            public delegate bool IsExtInPatcherHandle(string bundleName);

            private Dictionary<string, string[]> _depensMapping;
            private Dictionary<string, BundleAsyncOperation> _bundleLoadedDict;
            private List<BundleAsyncOperation> _bundleLoadingList;
            private List<BundleOperationCollection> _bundleOperationCollectionList;
            private Dictionary<string, string> _pathCacheDict;
            private IsExtInPatcherHandle mIsExtInPatcherHandle;

            private bool preloadDone = false;
            public override float GetPreloadProgress()
            {
                return preloadDone ? 1.0f : 0.0f;
            }

            protected override void onInitialization()
            {
                base.onInitialization();
                this._bundleLoadedDict = new Dictionary<string, BundleAsyncOperation>(1024);
                this._bundleLoadingList = new List<BundleAsyncOperation>(512);
            }

            protected override void onPreload()
            {
                base.onPreload();
                this.mFramework.mLifeCycle.mUpdateHandle.AddEventHandler(onTick);
                preloadDone = true;
            }

            public void SetDepensMapping(Dictionary<string, string[]> depensMapping)
            {
                _depensMapping = depensMapping;
            }
             
            public BundleOperationCollection LoadBundleAsync(string bundleName)
            {
                BundleOperationCollection bundleOperationCollection = BundleOperationCollection.New();
                if (this.tryGetBundleOperationInCache(bundleName, out bundleOperationCollection.mMainBundleAsyncOperation) == false)
                {
                    bundleOperationCollection.mMainBundleAsyncOperation = createBundleAsyncOperation(bundleName);
                }

                if (this._depensMapping.TryGetValue(bundleName, out string[] depensBundleName) == false)
                {
                    SnakeDebuger.ErrorFormat("无法在依赖列表中找到bundlename为{0}的数据，不应该存在此情况", bundleName);
                    return null;
                }

                if (depensBundleName.Length > 0)
                {
                    bundleOperationCollection.mDependAsyncOperation = new BundleAsyncOperation[depensBundleName.Length];
                    int len = depensBundleName.Length;
                    for (int i = 0; i < len; i++)
                    {
                        string depBundleName = depensBundleName[i];
                        if (this.tryGetBundleOperationInCache(depBundleName, out BundleAsyncOperation bundleAsyncOperation) == false)
                            bundleOperationCollection.mDependAsyncOperation[i] = createBundleAsyncOperation(bundleName);
                        else
                            bundleOperationCollection.mDependAsyncOperation[i] = bundleAsyncOperation;
                    }
                }

                return bundleOperationCollection;
            }

            public void Release(BundleOperationCollection bundleOperationCollection)
            {
                _bundleOperationCollectionList.Add(bundleOperationCollection);
            }

            private string operationBundleLoadPath(string bundleName)
            {
                if (_pathCacheDict.TryGetValue(bundleName, out var runtimePath) == false)
                {
                    runtimePath = Path.Combine(
                        mIsExtInPatcherHandle.Invoke(bundleName) ? SnakeDefine.Path.PERSISTENT_DATA_PATH : SnakeDefine.Path.STREAMING_ASSET_PATH,
                        bundleName);
                    _pathCacheDict[bundleName] = runtimePath;
                }
                return runtimePath;
            }

            private bool tryGetBundleOperationInCache(string bundleName, out BundleAsyncOperation bundleAsyncOperation)
            {
                if (this._bundleLoadedDict.TryGetValue(bundleName, out bundleAsyncOperation) == false)
                    return false;

                int index = this._bundleLoadingList.FindIndex(a => a.mBundleFullName == bundleName);
                if (index < 0)
                    return false;

                bundleAsyncOperation = this._bundleLoadingList[index];
                bundleAsyncOperation.AddReferenceCount();
                return true;
            }

            private BundleAsyncOperation createBundleAsyncOperation(string bundleName)
            {
                BundleAsyncOperation bundleAsyncOperation = BundleAsyncOperation.New(operationBundleLoadPath(bundleName), bundleName);
                bundleAsyncOperation.AddReferenceCount();
                this._bundleLoadingList.Add(bundleAsyncOperation);
                return bundleAsyncOperation;
            }

            private void releaseBundleAsyncOperation(BundleAsyncOperation bundleAsyncOperation)
            {
                if (this._bundleLoadedDict.TryGetValue(bundleAsyncOperation.mBundleFullName, out var tBundleAsyncOperation))
                    this._bundleLoadedDict.Remove(bundleAsyncOperation.mBundleFullName);

                int index = this._bundleLoadingList.FindIndex(a => a.mBundleFullName == bundleAsyncOperation.mBundleFullName);
                if (index >= 0)
                {
                    this._bundleLoadingList.RemoveAt(index);
                }
                BundleAsyncOperation.Release(bundleAsyncOperation);
            }

            private void onTick(int frameCount, float time, float deltaTime, float unscaledTime, float realtimeSinceStartup)
            {
                //释放BundleAsyncOperation
                for (int i = 0; i < _bundleOperationCollectionList.Count; i++)
                {
                    releaseBundleAsyncOperation(_bundleOperationCollectionList[i].mMainBundleAsyncOperation);
                    foreach (var a in _bundleOperationCollectionList[i].mDependAsyncOperation)
                        releaseBundleAsyncOperation(a);
                }
                _bundleOperationCollectionList.Clear();

                ///监控加载中的BundleAsyncOperation
                for (int i = 0; i < this._bundleLoadingList.Count; i++)
                {
                    BundleAsyncOperation bundleAsyncOperation = this._bundleLoadingList[i];
                    if (bundleAsyncOperation.GetIsDone() == false)
                        continue;
                    this._bundleLoadedDict.Add(bundleAsyncOperation.mBundleFullName, bundleAsyncOperation);
                    this._bundleLoadingList.RemoveAt(i--);
                }
            }
        }
    }
}