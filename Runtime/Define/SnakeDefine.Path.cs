using UnityEngine;

namespace com.snake.framework
{
    namespace runtime
    {
        public partial class SnakeDefine
        {
            public class Path
            {
                static private string _streamingAsset;
                static private string _persistentDataPath;

                public static string STREAMING_ASSET_PATH
                {
                    get
                    {
                        if (string.IsNullOrEmpty(_streamingAsset))
                        {
                            _streamingAsset = UnityEngine.Application.streamingAssetsPath;
                        }
                        return _streamingAsset;
                    }
                }

                public static string PERSISTENT_DATA_PATH
                {
                    get
                    {
                        if (string.IsNullOrEmpty(_persistentDataPath))
                        {
#if UNITY_EDITOR
                            _persistentDataPath = Application.dataPath + "/../mPersistentDataPath";
                            if (System.IO.Directory.Exists(_persistentDataPath) == false)
                                System.IO.Directory.CreateDirectory(_persistentDataPath);
#else
                        _persistentDataPath = UnityEngine.Application.persistentDataPath;
#endif
                        }
                        return _persistentDataPath;
                    }
                }
            }
        }
    }
}
