using System.Collections.Generic;
using System.Linq;

namespace com.snake.framework
{
    namespace runtime
    {
        /// <summary>
        /// 下载管理器
        /// </summary>
        public class DownloadManager : BaseManager
        {
            /// <summary>
            /// 下载任务列表
            /// </summary>
            private List<DownloadTask> _downloadingList;

            /// <summary>
            /// 下载速度刷新时间（秒）
            /// </summary>
            private float mUpdateDownloadSpeedInterval = 1;

            /// <summary>
            /// 并行下载数量，（默认5个）
            /// </summary>
            public int mMaxDownloadTaskCount = 5;

            /// <summary>
            /// 下载速度开关
            /// </summary>
            public bool mActiveDownloadSpeedMonitor { get; private set; } = false;

            /// <summary>
            /// 下载速度
            /// </summary>
            public long mDownloadSpeed { get; private set; } = 0;

            /// <summary>
            /// 下载的总大小
            /// </summary>
            public long mTotalDownloadSize { get; private set; } = 0;

            /// <summary>
            /// 是否下载中
            /// </summary>
            public bool mDownloading { get; private set; } = false;

            /// <summary>
            /// 完成所有下载任务的回调
            /// </summary>
            public System.Action mOnCompleted { get; private set; }

            //上一次记录下载速度时间
            private float _prevTickDownloadSpeedTime = 0;
            //上一次记录下载大小
            private long _prevTickDownloadedSize = 0;

            protected override void onInitialization()
            {
                this._downloadingList = new List<DownloadTask>();
            }

            /// <summary>
            /// 计算已经完成下载的大小
            /// </summary>
            public long GetCurrDownLoadSize()
            {
                long downloadingSize = 0;
                for (var i = 0; i < _downloadingList.Count; i++)
                {
                    DownloadTask task = _downloadingList[i];
                    switch (task.mState)
                    {
                        case DownloadTask.STATE.success:
                            downloadingSize += task.mTotalSize;
                            break;
                        case DownloadTask.STATE.downloading:
                            downloadingSize += (long)(task.mTotalSize * task.mProgress);
                            break;
                        default:
                            break;
                    }
                }
                return downloadingSize;
            }

            /// <summary>
            /// 下载速度监控开关
            /// </summary>
            public void SetDownloadSpeedMonitor(bool active)
            {
                if (mActiveDownloadSpeedMonitor == active)
                    return;
                mActiveDownloadSpeedMonitor = active;
            }

            /// <summary>
            /// 开始下载
            /// </summary>
            /// <param name="downloadFilePathQueue"></param>
            /// <param name="downloadingCallback"></param>
            /// <param name="finishCallback"></param>
            async public void StartDownload(string url, string savePath, int priority = 0)
            {
                if (mDownloading == false)
                {
                    mDownloading = true;
                    mFramework.mLifeCycle.mUpdateHandle.AddEventHandler(onDownloadProcess);
                }

                DownloadTask downloadTask = ReferencePool.Take<DownloadTask>();
                _downloadingList.Add(downloadTask);
                await downloadTask.SetDownloadInfo(url, savePath);
                this.mTotalDownloadSize += downloadTask.mTotalSize;
                if (priority > 0)
                {
                    downloadTask.mPriority = priority;
                    sortPriority();
                }
            }

            /// <summary>
            /// 获取下载出错的信息
            /// </summary>
            /// <returns></returns>
            public string[] GetDownloadErrorInfos()
            {
                if (this._downloadingList.Count == 0)
                    return new string[0];
                string[] errors = new string[this._downloadingList.Count];
                for (int i = 0; i < this._downloadingList.Count; i++)
                    errors[i] = this._downloadingList[i].mURL;
                return errors;
            }

            /// <summary>
            /// 获取已经获取web头，但未开始下载的数量
            /// </summary>
            /// <returns></returns>
            public int GetReadyTaskCount() 
            {
                if (this._downloadingList == null)
                    return 0;

                int count = 0;
                for (int i = 0; i < this._downloadingList.Count; i++)
                    count += this._downloadingList[i].mState == DownloadTask.STATE.ready ? 1 : 0;
                return count;
            }

            /// <summary>
            /// 下载过程监控
            /// </summary>
            /// <param name="frameCount"></param>
            /// <param name="time"></param>
            /// <param name="deltaTime"></param>
            /// <param name="unscaledTime"></param>
            /// <param name="realElapseSeconds"></param>
            private void onDownloadProcess(int frameCount, float time, float deltaTime, float unscaledTime, float realElapseSeconds)
            {
                int downloadCount = this._downloadingList.Sum(a => a.mState == DownloadTask.STATE.downloading ? 1 : 0);
                if (this.mMaxDownloadTaskCount > downloadCount)
                {
                    int index = _downloadingList.FindIndex(a => a.mState == DownloadTask.STATE.ready);
                    if (index >= 0)
                        _downloadingList[index].StartDownLoad();
                }

                //监控下载速度
                if (mActiveDownloadSpeedMonitor == true)
                    internalUpdateDownloadSpeed(realElapseSeconds);

                //如果全部完成，移除tick生命周期
                foreach (var a in _downloadingList)
                {
                    if (a.mIsDone == false)
                        return;
                }
                mFramework.mLifeCycle.mUpdateHandle.RemoveEventHandler(onDownloadProcess);

                this.mOnCompleted?.Invoke();

                //重置环境
                reset();
            }

            /// <summary>
            /// 优先级排序
            /// </summary>
            private void sortPriority()
            {
                _downloadingList.Sort((left, right) =>
                {
                    return left.mPriority.CompareTo(right.mPriority);
                });
            }

            /// <summary>
            /// 更新下载速度
            /// </summary>
            /// <param name="realElapseSeconds"></param>
            private void internalUpdateDownloadSpeed(float realElapseSeconds)
            {
                if (realElapseSeconds - _prevTickDownloadSpeedTime < 1.0f)
                    return;
                 
                long downloadedSize = GetCurrDownLoadSize();
                long detailSize = downloadedSize - _prevTickDownloadedSize;
                this.mDownloadSpeed = (long)(detailSize / mUpdateDownloadSpeedInterval);

                _prevTickDownloadSpeedTime = realElapseSeconds;
                _prevTickDownloadedSize = downloadedSize;
            }

            /// <summary>
            /// 重置环境
            /// </summary>
            private void reset() 
            {
                this._prevTickDownloadSpeedTime = 0;
                this._prevTickDownloadedSize = 0;

                _downloadingList?.Clear();
                mOnCompleted = null;
                mDownloading = false;
                mActiveDownloadSpeedMonitor = false;
                mDownloadSpeed = 0;
                mTotalDownloadSize = 0;
            }
        }
    }
}