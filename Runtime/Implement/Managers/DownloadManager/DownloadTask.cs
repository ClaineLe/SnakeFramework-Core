using System.Threading.Tasks;
using UnityEngine.Networking;

namespace com.snake.framework
{
    namespace runtime
    {
        /// <summary>
        /// 下载任务
        /// </summary>
        internal class DownloadTask : IReference
        {
            /// <summary>
            /// 下载器任务状态
            /// </summary>
            public enum STATE
            {
                /// <summary>
                /// 无
                /// </summary>
                none,
                
                /// <summary>
                /// 初始化
                /// </summary>
                init,
                
                /// <summary>
                /// 初始化完成，可进行下载
                /// </summary>
                ready,
                
                /// <summary>
                /// 下载成功
                /// </summary>
                success,
                
                /// <summary>
                /// 下载失败
                /// </summary>
                fail,
              
                /// <summary>
                /// 下载中
                /// </summary>
                downloading,
            }

            /// <summary>
            /// 头-内容长度
            /// </summary>
            private const string CONTENT_LENGTH = "Content-Length";

            /// <summary>
            /// 当前下载状态
            /// </summary>
            public STATE mState { get; private set; } = STATE.none;

            /// <summary>
            /// 下载优先级
            /// </summary>
            public int mPriority;

            /// <summary>
            /// 错误信息
            /// </summary>
            public string mError { get; private set; } = string.Empty;

            /// <summary>
            /// 下载信息
            /// </summary>
            public string mURL { get; private set; } = string.Empty;

            /// <summary>
            /// 本地保存地址
            /// </summary>
            public string mSavePath { get; private set; } = string.Empty;

            /// <summary>
            /// 文件大小
            /// </summary>
            public long mTotalSize { get; private set; } = 0;

            /// <summary>
            /// 是否完成
            /// </summary>
            public bool mIsDone { get; private set; } = false;

            /// <summary>
            /// 下载请求
            /// </summary>
            private UnityWebRequest _request;

            /// <summary>
            /// 下载进度
            /// </summary>
            public float mProgress => _request == null ? 0 : _request.downloadProgress;

            /// <summary>
            /// 设置下载信息
            /// </summary>
            /// <param name="url"></param>
            /// <param name="savePath"></param>
            async public Task SetDownloadInfo(string url, string savePath)
            {
                this.mURL = url;
                this.mSavePath = savePath;
                this.mState = STATE.init;
                await Utility.Web.GetResponseHeader(url, CONTENT_LENGTH, 0, (result, lengthStr) =>
                {
                    if (result != UnityWebRequest.Result.Success)
                    {
                        Debuger.Error("获取Web文件头大小异常。\nURL:" + url + "\nerrorInfo:" + result);
                        return;
                    }

                    long totalSize;
                    if (long.TryParse(lengthStr, out totalSize) == false)
                    {
                        Debuger.Error("解析Web文件头大小异常。URL:" + url + ", into:" + lengthStr);
                        return;
                    }
                    this.mTotalSize = totalSize;
                    this.mState = STATE.ready;
                });
            }

            /// <summary>
            /// 开始下载
            /// </summary>
            async public void StartDownLoad(int timeout = 0)
            {
                _request = UnityWebRequest.Get(this.mURL);
                if (timeout > 0)
                    this._request.timeout = timeout;
                _request.downloadHandler = new DownloadHandlerFile(this.mSavePath, false);
                ((DownloadHandlerFile)_request.downloadHandler).removeFileOnAbort = true;
                this.mState = STATE.downloading;
                await _request.SendWebRequest();
                if (_request.result != UnityWebRequest.Result.Success)
                {
                    mError = $"下载文件错误\nErrorMsg:{_request.error}  Result:{_request.result} \nurl:{this.mSavePath}";
                    this.mState = STATE.fail;
                }
                else
                {
                    this.mState = STATE.success;
                }
                this.mIsDone = true;
            }

            /// <summary>
            /// 对象回退对象池
            /// </summary>
            public void OnReferenceClear()
            {
                this.mState = STATE.none;
                this.mPriority = 0;
                this.mError = string.Empty;
                this.mURL = string.Empty;
                this.mSavePath = string.Empty;
                this.mTotalSize = 0;
                this.mIsDone = false;

                if (_request != null)
                {
                    _request.Dispose();
                    _request = null;
                }
            }
        }
    }
}