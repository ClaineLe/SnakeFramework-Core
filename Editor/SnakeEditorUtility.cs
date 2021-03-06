using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.snake.framework 
{
    namespace editor 
    {
        public class SnakeEditorUtility
        {
            static public string GetPackagesPath()
            {
                string packagePath = "Packages/com.snake.framework.core";
                if (Directory.Exists(packagePath))
                {
                    return packagePath;
                }
                return "Assets/UnityPackages/com.snake.framework.core";
            }
        }
    }
}