using System.Collections.Generic;

namespace com.snake.framework
{
    namespace runtime
    {
        [System.Serializable]
        public class AssetBundleCatalog
        {
            //AssetPath - AssetBundleName
            public Dictionary<string, string> mPathMapping = new Dictionary<string, string>();

            //AssetBundleName - Depens
            public Dictionary<string, string[]> mDepensMapping = new Dictionary<string, string[]>();
        }
    }
}