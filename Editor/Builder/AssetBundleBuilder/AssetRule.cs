using System.IO;
using UnityEngine;

namespace com.snake.framework
{
    namespace editor
    {
        public enum PACKER_MODE
        {
            single,
            together,
            childfold,
        }

        [CreateAssetMenu()]
        public class AssetRule : ScriptableObject
        {
            public int priority;
            public string foldPath;
            public PACKER_MODE packerMode;
            public string[] types;
            public string[] filters;
        }
    }
}