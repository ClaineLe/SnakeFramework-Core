using UnityEngine;

namespace com.snake.framework
{
    namespace runtime
    {
        public abstract class SnakeBasicSetting<T> : UnityEngine.ScriptableObject
            where T : UnityEngine.Object
        {
            static public T Get()
            {
                T setting = Resources.Load<T>(typeof(T).Name);
                if (setting == null)
                {
                    SnakeDebuger.ErrorFormat("没有在Resources目录下找到{0}.asset，请右键创建一个", typeof(T).Name);
                    return null;
                }
                return setting;
            }

#if UNITY_EDITOR
            static protected T editorGet(string editorPath) 
            {
                T setting = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(editorPath);
                if (setting == null)
                {
                    SnakeDebuger.ErrorFormat("没有找到路径{0}，请右键创建一个", editorPath);
                    return null;
                }
                return setting;
            }
#endif
        }
    }
}