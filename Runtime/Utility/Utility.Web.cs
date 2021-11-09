using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace com.snake.framework
{
    namespace runtime
    {
        public static partial class Utility
        {
            public class Web
            {
                /// <summary>
                /// Get方式访问http
                /// </summary>
                /// <param name="pUrl"></param>
                /// <param name="pTimeout"></param>
                /// <param name="pCallback"></param>
                public static async Task<UnityWebRequest.Result> Get(string uri, int timeout = 0, Action<UnityWebRequest.Result> callback = null)
                {
                    using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(uri))
                    {
                        if (timeout > 0)
                            unityWebRequest.timeout = timeout;
                        await unityWebRequest.SendWebRequest();
                        UnityWebRequest.Result result = unityWebRequest.result;
                        callback?.Invoke(result);
                        return result;
                    }
                }

                /// <summary>
                /// Get方式访问http
                /// </summary>
                /// <param name="pUrl"></param>
                /// <param name="pType">传类型的时候会序列化成该类型的对象 传byte[]或者string会返回对应对象 传其他则会走json反序列化</param>
                /// <param name="pCallback"></param>
                public static async Task<UnityWebRequest.Result> Get(string uri, Type dataType, int timeout = 0, Action<UnityWebRequest.Result, object> callback = null)
                {
                    using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(uri))
                    {
                        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                        if (timeout > 0)
                            unityWebRequest.timeout = timeout;
                        await unityWebRequest.SendWebRequest();
                        UnityWebRequest.Result result = unityWebRequest.result;

                        if (result != UnityWebRequest.Result.Success)
                        {
                            callback?.Invoke(result, null);
                            return result;
                        }

                        if (dataType == typeof(string))
                        {
                            callback?.Invoke(result, unityWebRequest.downloadHandler.text);
                            return result;
                        }

                        if (dataType == typeof(byte[]))
                        {
                            callback?.Invoke(result, unityWebRequest.downloadHandler.data);
                            return result;
                        }


                        if (dataType != null)
                        {
                            var content = unityWebRequest.downloadHandler.text;
                            var jsonObj = Utility.Json.FromJson(content, dataType);
                            callback?.Invoke(result, jsonObj);
                            return result;
                        }

                        callback?.Invoke(result, unityWebRequest.downloadHandler.data);
                        return result;
                    }
                }

                /// <summary>
                /// Post方式访问http
                /// </summary>
                /// <param name="pUrl"></param>
                /// <param name="pTimeout"></param>
                /// <param name="pCallback"></param>
                [Obsolete("use post with param byte[] instead")]
                public static async Task<UnityWebRequest.Result> Post(string uri, string postData, int timeout = 0, Action<UnityWebRequest.Result, string> callback = null)
                {
                    using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(uri, postData))
                    {
                        if (timeout > 0)
                            unityWebRequest.timeout = timeout;
                        await unityWebRequest.SendWebRequest();
                        UnityWebRequest.Result result = unityWebRequest.result;
                        var text = string.Empty;
                        if (unityWebRequest.downloadHandler != null)
                        {
                            text = unityWebRequest.downloadHandler.text;
                        }
                        callback?.Invoke(result, text);
                        return result;
                    }
                }

                /// <summary>
                /// Post方式访问http
                /// </summary>
                /// <param name="pUrl"></param>
                /// <param name="pTimeout"></param>
                /// <param name="pCallback"></param>
                [Obsolete("use post with param byte[] instead")]
                public static async Task<UnityWebRequest.Result> Post(string uri, Dictionary<string, string> formDataDict, int timeout = 0, Action<UnityWebRequest.Result> callback = null)
                {
                    using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(uri, formDataDict))
                    {
                        if (timeout > 0)
                            unityWebRequest.timeout = timeout;
                        await unityWebRequest.SendWebRequest();
                        UnityWebRequest.Result result = unityWebRequest.result;
                        callback?.Invoke(result);
                        return result;
                    }
                }

                /// <summary>
                /// Post方式访问
                /// </summary>
                /// <param name="pUrl"></param>
                /// <param name="pFormDataDict"></param>
                /// <param name="pTimeout"></param>
                /// <param name="type">传类型的时候会序列化成该类型的对象 传byte[]或者string会返回对应对象 传其他则会走json反序列化</param>
                /// <param name="pCallback"></param>
                [Obsolete("use post with param byte[] instead")]
                public static async Task<UnityWebRequest.Result> Post(string uri, Dictionary<string, string> formDataDict, Type dataType, int timeout = 0, Action<UnityWebRequest.Result, object> callback = null)
                {
                    using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(uri, formDataDict))
                    {
                        if (timeout > 0)
                            unityWebRequest.timeout = timeout;
                        await unityWebRequest.SendWebRequest();
                        UnityWebRequest.Result result = unityWebRequest.result;
                        if (result != UnityWebRequest.Result.Success)
                        {
                            callback?.Invoke(result, null);
                            return result;
                        }

                        if (dataType == typeof(byte[]))
                        {
                            callback?.Invoke(result, unityWebRequest.downloadHandler.data);
                            return result;
                        }

                        if (dataType == typeof(string))
                        {
                            callback?.Invoke(result, unityWebRequest.downloadHandler.text);
                            return result;
                        }

                        var content = unityWebRequest.downloadHandler.text;
                        var jsonObj = Utility.Json.FromJson(content, dataType);
                        callback?.Invoke(result, jsonObj);
                        return result;
                    }
                }

                /// <summary>
                /// post
                /// </summary>
                /// <param name="uri"></param>
                /// <param name="headerDict"></param>
                /// <param name="content"></param>
                /// <param name="timeout"></param>
                /// <param name="callback"></param>
                public static async void PostWithCallBack(string uri, Dictionary<string, string> headerDict, byte[] content, int timeout = 5, Action<byte[]> callback = null)
                {
                    var postTask = Post(uri, headerDict, content, timeout);
                    await postTask;
                    callback?.Invoke(postTask.Result);
                }

                /// <summary>
                /// post请求
                /// </summary>
                /// <param name="uri"></param>
                /// <param name="headerDict"></param>
                /// <param name="content"></param>
                /// <param name="timeout"></param>
                /// <returns></returns>
                public static async Task<byte[]> Post(string uri, Dictionary<string, string> headerDict, byte[] content, int timeout = 5)
                {
                    using UnityWebRequest request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
                    if (timeout > 0)
                    {
                        request.timeout = timeout;
                    }
                    if (headerDict != null)
                    {
                        var enumerator = headerDict.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            request.SetRequestHeader(enumerator.Current.Key, enumerator.Current.Value);
                        }
                    }
                    if (content != null)
                    {
                        request.uploadHandler = new UploadHandlerRaw(content);
                    }
                    request.downloadHandler = new DownloadHandlerBuffer();
                    await request.SendWebRequest();
                    return request.downloadHandler.data;
                }

                /// <summary>
                /// 请求头
                /// </summary>
                /// <param name="url"></param>
                /// <param name="timeOut"></param>
                /// <param name="callback"></param>
                /// <returns></returns>
                public static async Task<UnityWebRequest.Result> GetResponseHeader(string uri, string headName, int timeOut = 0, Action<UnityWebRequest.Result, string> callback = null)
                {
                    using (UnityWebRequest unityWebRequest = UnityWebRequest.Head(uri))
                    {
                        if (timeOut > 0)
                            unityWebRequest.timeout = timeOut;
                        await unityWebRequest.SendWebRequest();
                        UnityWebRequest.Result result = unityWebRequest.result;
                        if (result != UnityWebRequest.Result.Success)
                        {
                            callback?.Invoke(result, null);
                            return result;
                        }
                        string headValue = unityWebRequest.GetResponseHeader(headName);
                        callback?.Invoke(result, headValue);
                        return result;
                    }
                }

                /// <summary>
                /// 请求所有头
                /// </summary>
                /// <param name="url"></param>
                /// <param name="timeOut"></param>
                /// <param name="callback"></param>
                /// <returns></returns>
                public static async Task<UnityWebRequest.Result> GetResponseHeaders(string uri, int timeOut = 0, Action<UnityWebRequest.Result, Dictionary<string, string>> callback = null)
                {
                    using (UnityWebRequest unityWebRequest = UnityWebRequest.Head(uri))
                    {
                        if (timeOut > 0)
                            unityWebRequest.timeout = timeOut;
                        await unityWebRequest.SendWebRequest();
                        UnityWebRequest.Result result = unityWebRequest.result;
                        if (result != UnityWebRequest.Result.Success)
                        {
                            callback?.Invoke(result, null);
                            return result;
                        }
                        Dictionary<string, string> headDict = unityWebRequest.GetResponseHeaders();
                        callback?.Invoke(result, headDict);
                        return result;
                    }
                }
            }
        }
    }
}