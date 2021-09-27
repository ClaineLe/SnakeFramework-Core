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
            /// Get方式访问http
            /// </summary>
            /// <param name="pUrl"></param>
            /// <param name="pTimeout"></param>
            /// <param name="pCallback"></param>
            public static async Task<UnityWebRequest.Result> Get(string pUrl, Action<UnityWebRequest.Result> pCallback, int pTimeout)
            {
                UnityWebRequest unityWebRequest = UnityWebRequest.Get(pUrl);
                unityWebRequest.timeout = pTimeout;
                await unityWebRequest.SendWebRequest();
                UnityWebRequest.Result result = unityWebRequest.result;
                pCallback?.Invoke(result);
                unityWebRequest.Dispose();
                return result;
            }

            /// <summary>
            /// Get方式访问http
            /// </summary>
            /// <param name="pUrl"></param>
            /// <param name="pType">传类型的时候会序列化成该类型的对象 传byte[]或者string会返回对应对象 传其他则会走json反序列化</param>
            /// <param name="pCallback"></param>
            public static async Task<UnityWebRequest.Result> Get(string pUrl, int pTimeout, Type pType, Action<UnityWebRequest.Result, object> pCallback)
            {
                UnityWebRequest unityWebRequest = UnityWebRequest.Get(pUrl);
                unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                unityWebRequest.timeout = pTimeout;
                await unityWebRequest.SendWebRequest();
                UnityWebRequest.Result result = unityWebRequest.result;

                if (result != UnityWebRequest.Result.Success)
                {
                    pCallback?.Invoke(result, null);
                    unityWebRequest.Dispose();
                    return result;
                }

                if (pType == typeof(string))
                {
                    pCallback?.Invoke(result, unityWebRequest.downloadHandler.text);
                    unityWebRequest.Dispose();
                    return result;
                }

                if (pType != null)
                {
                    var content = unityWebRequest.downloadHandler.text;
                    var jsonObj = com.snake.framework.Utility.Json.FromJson(content, pType);
                    pCallback?.Invoke(result, jsonObj);
                    unityWebRequest.Dispose();
                    return result;
                }

                pCallback?.Invoke(result, unityWebRequest.downloadHandler.data);
                unityWebRequest.Dispose();
                return result;
            }


            /// <summary>
            /// Post方式访问http
            /// </summary>
            /// <param name="pUrl"></param>
            /// <param name="pTimeout"></param>
            /// <param name="pCallback"></param>
            public static async Task<UnityWebRequest.Result> Post(string pUrl, string postData, Action<UnityWebRequest.Result, string> pCallback, int pTimeout)
            {
                UnityWebRequest unityWebRequest = UnityWebRequest.Post(pUrl, postData);
                unityWebRequest.timeout = pTimeout;
                await unityWebRequest.SendWebRequest();
                UnityWebRequest.Result result = unityWebRequest.result;
                var text = string.Empty;
                if (unityWebRequest.downloadHandler != null)
                {
                    text = unityWebRequest.downloadHandler.text;
                }
                pCallback?.Invoke(result, text);
                unityWebRequest.Dispose();
                return result;
            }

            /// <summary>
            /// Post方式访问http
            /// </summary>
            /// <param name="pUrl"></param>
            /// <param name="pTimeout"></param>
            /// <param name="pCallback"></param>
            public static async Task<UnityWebRequest.Result> Post(string pUrl, Dictionary<string, string> pFormDataDict, Action<UnityWebRequest.Result> pCallback, int pTimeout)
            {
                UnityWebRequest unityWebRequest = UnityWebRequest.Post(pUrl, pFormDataDict);
                unityWebRequest.timeout = pTimeout;
                await unityWebRequest.SendWebRequest();
                UnityWebRequest.Result result = unityWebRequest.result;
                pCallback?.Invoke(result);
                unityWebRequest.Dispose();
                return result;
            }

            /// <summary>
            /// Post方式访问
            /// </summary>
            /// <param name="pUrl"></param>
            /// <param name="pFormDataDict"></param>
            /// <param name="pTimeout"></param>
            /// <param name="type">传类型的时候会序列化成该类型的对象 传byte[]或者string会返回对应对象 传其他则会走json反序列化</param>
            /// <param name="pCallback"></param>
            public static async Task<UnityWebRequest.Result> Post(string pUrl, Dictionary<string, string> pFormDataDict, Type type, Action<UnityWebRequest.Result, object> pCallback, int pTimeout)
            {
                UnityWebRequest unityWebRequest = UnityWebRequest.Post(pUrl, pFormDataDict);
                unityWebRequest.timeout = pTimeout;
                await unityWebRequest.SendWebRequest();
                UnityWebRequest.Result result = unityWebRequest.result;
                if (result != UnityWebRequest.Result.Success)
                {
                    pCallback?.Invoke(result, null);
                    unityWebRequest.Dispose();
                    return result;
                }

                if (type == typeof(byte[]))
                {
                    pCallback?.Invoke(result, unityWebRequest.downloadHandler.data);
                    unityWebRequest.Dispose();
                    return result;
                }

                if (type == typeof(string))
                {
                    pCallback?.Invoke(result, unityWebRequest.downloadHandler.text);
                    unityWebRequest.Dispose();
                    return result;
                }

                var content = unityWebRequest.downloadHandler.text;
                var jsonObj = com.snake.framework.Utility.Json.FromJson(content, type);
                pCallback?.Invoke(result, jsonObj);
                unityWebRequest.Dispose();
                return result;
            }
        }
    }
}