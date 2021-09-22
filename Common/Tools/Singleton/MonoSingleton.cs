using UnityEngine;
namespace com.snake.framework
{
    [AutoSingleton(true)]
    public class MonoSingleton<T> : MonoBehaviour where T : Component, ISingleton
    {
        private static T _instance;

        private static bool _destroyed;

        public static T instance
        {
            get
            {
                return MonoSingleton<T>.GetInstance();
            }
        }

        public static T GetInstance()
        {
            if (MonoSingleton<T>._instance == null && !MonoSingleton<T>._destroyed)
            {
                System.Type typeFromHandle = typeof(T);
                MonoSingleton<T>._instance = (T)((object)Object.FindObjectOfType(typeFromHandle));
                if (MonoSingleton<T>._instance == null)
                {
                    object[] customAttributes = typeFromHandle.GetCustomAttributes(typeof(AutoSingletonAttribute), true);
                    if (customAttributes.Length > 0 && !((AutoSingletonAttribute)customAttributes[0]).bAutoCreate)
                    {
                        return (T)((object)null);
                    }
                    GameObject gameObject = new GameObject(typeof(T).Name);
                    MonoSingleton<T>._instance = gameObject.AddComponent<T>();
                    GameObject bootObj = GameObject.Find("BootObj");
                    if (bootObj != null)
                    {
                        gameObject.transform.SetParent(bootObj.transform);
                    }
                }
            }
            return MonoSingleton<T>._instance;
        }

        public static void DestroyInstance()
        {
            if (MonoSingleton<T>._instance != null)
            {
                UnityEngine.Object.Destroy(MonoSingleton<T>._instance.gameObject);
            }
            MonoSingleton<T>._destroyed = true;
            MonoSingleton<T>._instance = (T)((object)null);
        }

        public static void ClearDestroy()
        {
            MonoSingleton<T>.DestroyInstance();
            MonoSingleton<T>._destroyed = false;
        }

        protected virtual void Awake()
        {
            if (MonoSingleton<T>._instance != null && MonoSingleton<T>._instance.gameObject != base.gameObject)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(base.gameObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(base.gameObject);
                }
            }
            else if (MonoSingleton<T>._instance == null)
            {
                MonoSingleton<T>._instance = base.GetComponent<T>();
            }
            Object.DontDestroyOnLoad(base.gameObject);
            this.Init();
        }

        protected virtual void OnDestroy()
        {
            if (MonoSingleton<T>._instance != null && MonoSingleton<T>._instance.gameObject == base.gameObject)
            {
                MonoSingleton<T>._instance = (T)((object)null);
            }
        }

        public static bool HasInstance()
        {
            return MonoSingleton<T>._instance != null;
        }

        protected virtual void Init()
        {
        }
    }
}
