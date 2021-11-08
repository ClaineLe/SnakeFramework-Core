using UnityEditor;
using UnityEngine;
using com.snake.framework.runtime;
using System.IO;
namespace com.snake.framework
{
    namespace editor
    {
        public class AssetRuleDraw
        {
            private string assetRulePath;
            public bool mFoldout;
            public string mName { get; private set; }
            public AssetRule mAssetRule { get; private set; }
            public SerializedObject mSerializedObject { get; private set; }
            public bool mRenameing = false;
            public bool modifyFoldPath = false;
            public string mAssetRulePath
            {
                get
                {
                    return assetRulePath;
                }
                set
                {
                    this.assetRulePath = value;
                    this.mName = Path.GetFileNameWithoutExtension(this.assetRulePath);
                }
            }

            public AssetRuleDraw(string assetRulePath)
            {
                this.mAssetRulePath = assetRulePath;
                ReloadAssetRule();
            }

            public void ReloadAssetRule()
            {

                if (this.mSerializedObject != null)
                {
                    this.mSerializedObject.Dispose();
                    this.mSerializedObject = null;
                }
                if (this.mAssetRule != null)
                {
                    Object.Destroy(this.mAssetRule);
                    this.mAssetRule = null;
                }

                this.mAssetRule = AssetDatabase.LoadAssetAtPath<AssetRule>(this.mAssetRulePath);
                if (this.mAssetRule == null)
                {
                    SnakeDebuger.Error("没有找到资源规则文件.path:" + this.mAssetRulePath);
                    return;
                }
                this.mSerializedObject = new SerializedObject(this.mAssetRule);
            }
        }
    }
}
