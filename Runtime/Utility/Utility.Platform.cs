namespace com.snake.framework
{
    namespace runtime
    {
        public static partial class Utility
        {
            public class Platform
            {
                public const string Android = "Android";
                public const string iOS = "iOS";
                public const string Windows = "Windows";
                public const string OSX = "OSX";


#if UNITY_EDITOR
                static public bool Editor = true;
#else
            static public bool Editor = false;
#endif

                public static bool IsIOS()
                {
                    return GetPlatformName().Equals(iOS);
                }

                public static string GetPlatformName()
                {
#if UNITY_EDITOR
                    switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
                    {
                        case UnityEditor.BuildTarget.Android:
                            return Android;
                        case UnityEditor.BuildTarget.iOS:
                            return iOS;
                        case UnityEditor.BuildTarget.StandaloneWindows:
                        case UnityEditor.BuildTarget.StandaloneWindows64:
                            return Windows;
                        case UnityEditor.BuildTarget.StandaloneOSX:
                            return OSX;
                        default:
                            return null;
                    }
#else
              switch (UnityEngine.Application.platform)
                {
                    case UnityEngine.RuntimePlatform.Android:
                        return "Android";
                    case UnityEngine.RuntimePlatform.IPhonePlayer:
                        return "iOS";
                    case UnityEngine.RuntimePlatform.WindowsPlayer:
                        return "Windows";
                    case UnityEngine.RuntimePlatform.OSXPlayer:
                        return "OSX";
                    default:
                        return null;
                }
#endif
                }
            }
        }
    }
}