using Newtonsoft.Json;
using System;

namespace com.snake.framework
{
    namespace runtime
    {
        public static partial class Utility
        {
            public class Json
            {
                static public T FromJson<T>(string json)
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }

                static public object FromJson(string json, Type type)
                {
                    return JsonConvert.DeserializeObject(json, type);
                }


                static public string ToJson(object target)
                {
                    return ToJson(target, Formatting.None);
                }

                static public string ToJson(object target, Formatting formatting)
                {
                    return JsonConvert.SerializeObject(target, formatting);
                }

                static public void ToFile(string path, object target)
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
                    if (fileInfo.Exists == true)
                        fileInfo.Delete();
                    if (fileInfo.Directory.Exists == false)
                        fileInfo.Directory.Create();
                    System.IO.File.WriteAllText(fileInfo.FullName, ToJson(target));
                }

                static public bool TryFromFile<T>(string path, out T target)
                {
                    target = default;
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
                    if (fileInfo.Exists == false)
                    {
                        return false;
                    }
                    string json = System.IO.File.ReadAllText(fileInfo.FullName);
                    target = FromJson<T>(json);
                    return true;
                }
            }
        }
    }
}