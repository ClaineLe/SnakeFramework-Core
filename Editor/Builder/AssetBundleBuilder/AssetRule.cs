using System.IO;
using UnityEngine;

namespace com.snake.framework
{
    namespace editor
    {
        [CreateAssetMenu()]
        public class AssetRule : ScriptableObject
        {
            public int priority;
            public string foldPath;
            public bool single;
            public string[] types;
            public string[] filters;
        }
    }
}