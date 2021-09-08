using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace com.snake.framework
{
    public static partial class Utility
    {

        public class Web
        {
            /// <summary>
            /// 错误处理
            /// </summary>
            /// <param name="pUWR"></param>
            /// <returns></returns>
            private static string ErrorHandle(UnityWebRequest pUWR)
            {
                string error = string.Empty;
                bool isNetworkError = pUWR.result == UnityWebRequest.Result.ConnectionError;
                bool isHttpError = pUWR.result == UnityWebRequest.Result.ProtocolError;
                if (isNetworkError || isHttpError)
                {
                    error = $"HttpGet网络错误\nErrorMsg:{pUWR.error} Type:{pUWR} isNetworkError:{isNetworkError} isHttpError:{isHttpError}\nurl:{pUWR.url}";
                }
                if (!pUWR.isDone)
                {
                    error = $"HttpGet访问超时\nErrorMsg:{pUWR.error} Type:{pUWR} isDone:{pUWR.isDone} \nurl:{pUWR.url}";
                }
                return error;
            }

            /// <summary>
            /// Get方式访问http
            /// </summary>
            /// <param name="pUrl"></param>
            /// <param name="pTimeout"></param>
            /// <param name="pCallback"></param>
            public static async Task<string> Get(string pUrl, int pTimeout, Action<string> pCallback)
            {
                var uwr = UnityWebRequest.Get(pUrl);
                uwr.timeout = pTimeout;
                await uwr.SendWebRequest();
                var code = ErrorHandle(uwr);
                pCallback?.Invoke(code);
                uwr.Dispose();
                return code;
            }

            /// <summary>
            /// Get方式访问http
            /// </summary>
            /// <param name="pUrl"></param>
            /// <param name="pType">传类型的时候会序列化成该类型的对象 传byte[]或者string会返回对应对象 传其他则会走json反序列化</param>
            /// <param name="pCallback"></param>
            public static async Task<string> Get(string pUrl, int pTimeout, Type pType, Action<string, object> pCallback)
            {
                var uwr = UnityWebRequest.Get(pUrl);
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.timeout = pTimeout;
                await uwr.SendWebRequest();
                string error = ErrorHandle(uwr);
                if (string.IsNullOrEmpty(error) == false)
                {
                    pCallback?.Invoke(error, null);
                    uwr.Dispose();
                    return error;
                }
                if (pType == typeof(byte[]))
                {
                    pCallback?.Invoke(error, uwr.downloadHandler.data);
                    uwr.Dispose();
                    return error;
                }
                if (pType == typeof(string))
                {
                    pCallback?.Invoke(error, uwr.downloadHandler.text);
                    uwr.Dispose();
                    return error;
                }
                if (pType != null)
                {
                    var content = uwr.downloadHandler.text;
                    var jsonObj = Utility.Json.FromJson(content, pType);
                    pCallback?.Invoke(error, jsonObj);
                    uwr.Dispose();
                    return error;
                }
                pCallback?.Invoke(error, uwr.downloadHandler.data);
                uwr.Dispose();
                return error;
            }


            /// <summary>
            /// Post方式访问http
            /// </summary>
            /// <param name="pUrl"></param>
            /// <param name="pTimeout"></param>
            /// <param name="pCallback"></param>
            public static async Task<string> Post(string pUrl, string postData, int pTimeout, Action<string, string> pCallback)
            {
                var uwr = UnityWebRequest.Post(pUrl, postData);
                uwr.timeout = pTimeout;
                await uwr.SendWebRequest();
                var code = ErrorHandle(uwr);

                var text = string.Empty;
                if (uwr.downloadHandler != null)
                {
                    text = uwr.downloadHandler.text;
                }

                pCallback?.Invoke(code, text);
                uwr.Dispose();
                return code;
            }

            /// <summary>
            /// Post方式访问http
            /// </summary>
            /// <param name="pUrl"></param>
            /// <param name="pTimeout"></param>
            /// <param name="pCallback"></param>
            public static async Task<string> Post(string pUrl, Dictionary<string, string> pFormDataDict, int pTimeout, Action<string> pCallback)
            {
                var uwr = UnityWebRequest.Post(pUrl, pFormDataDict);
                uwr.timeout = pTimeout;
                await uwr.SendWebRequest();
                var code = ErrorHandle(uwr);
                pCallback?.Invoke(code);
                uwr.Dispose();
                return code;
            }

            /// <summary>
            /// Post方式访问
            /// </summary>
            /// <param name="pUrl"></param>
            /// <param name="pFormDataDict"></param>
            /// <param name="pTimeout"></param>
            /// <param name="type">传类型的时候会序列化成该类型的对象 传byte[]或者string会返回对应对象 传其他则会走json反序列化</param>
            /// <param name="pCallback"></param>
            public static async Task<string> Post(string pUrl, Dictionary<string, string> pFormDataDict, int pTimeout, Type type, Action<string, object> pCallback)
            {
                var uwr = UnityWebRequest.Post(pUrl, pFormDataDict);
                uwr.timeout = pTimeout;
                await uwr.SendWebRequest();
                string error = ErrorHandle(uwr);
                if (string.IsNullOrEmpty(error) == false)
                {
                    pCallback?.Invoke(error, null);
                    uwr.Dispose();
                    return error;
                }
                if (type == typeof(byte[]))
                {
                    pCallback?.Invoke(error, uwr.downloadHandler.data);
                    uwr.Dispose();
                    return error;
                }
                if (type == typeof(string))
                {
                    pCallback?.Invoke(error, uwr.downloadHandler.text);
                    uwr.Dispose();
                    return error;
                }
                var content = uwr.downloadHandler.text;
                var jsonObj = Utility.Json.FromJson(content, type);
                pCallback?.Invoke(error, jsonObj);
                uwr.Dispose();
                return error;
            }
        }
    }
}