using System.IO;

namespace com.snake.framework
{
    namespace runtime
    {
        public static partial class Utility
        {
            public class Fold
            {


#if UNITY_EDITOR
                static public void ClearFold(string foldPath, string[] ignores = null)
                {
                    DirectoryInfo foldInfo = new DirectoryInfo(foldPath);
                    if (foldInfo.Exists == false)
                        return;

                    FileSystemInfo[] fileSystemInfos = foldInfo.GetFileSystemInfos("*", SearchOption.AllDirectories);
                    if (fileSystemInfos == null || fileSystemInfos.Length <= 0)
                        return;

                    for (int i = 0; i < fileSystemInfos.Length; i++)
                    {
                        if (fileSystemInfos[i].Exists == false)
                            continue;

                        string fullPath = fileSystemInfos[i].FullName;
                        if (_isExists(fullPath, ignores))
                            continue;

                        UnityEditor.FileUtil.DeleteFileOrDirectory(fileSystemInfos[i].FullName.FixSlash());
                    }
                }

                static public void CopyFold(string fromPath, string toPath, string[] ignores = null)
                {
                    DirectoryInfo foldInfo = new DirectoryInfo(fromPath);
                    if (foldInfo.Exists == false)
                        return;

                    FileSystemInfo[] fileSystemInfos = foldInfo.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
                    if (fileSystemInfos == null || fileSystemInfos.Length <= 0)
                        return;

                    for (int i = 0; i < fileSystemInfos.Length; i++)
                    {

                        if (fileSystemInfos[i].Exists == false)
                            continue;

                        string fullPath = fileSystemInfos[i].FullName;
                        if (_isExists(fullPath, ignores))
                            continue;

                        string fileName = fileSystemInfos[i].Name;

                        UnityEditor.FileUtil.CopyFileOrDirectory(fromPath + "/" + fileName, toPath + "/" + fileName);
                    }
                }
#endif

                static private bool _isExists(string path, string[] ignores = null)
                {
                    if (ignores == null || ignores.Length <= 0)
                        return false;

                    for (int i = 0; i < ignores.Length; i++)
                    {
                        if (path.Contains(ignores[i].FixSlash()) == true)
                            return true;
                    }
                    return false;
                }

                /// <summary>
                /// 获取目录下文件
                /// </summary>
                /// <param name="foldPath">目录路径</param>
                /// <param name="searchOption">搜索配置</param>
                /// <param name="filters">筛选</param>
                /// <param name="ignores">忽略</param>
                /// <returns></returns>
                static public FileInfo[] GetFiles(string foldPath, SearchOption searchOption, string[] filters, string[] ignores)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(foldPath);
                    System.Collections.Generic.List<FileInfo> fileInfoList = new System.Collections.Generic.List<FileInfo>(dirInfo.GetFiles("*", searchOption));

                    if (filters != null && filters.Length > 0)
                    {
                        foreach (string filter in filters)
                        {
                            fileInfoList.RemoveAll(a => a.FullName.Contains(filter) == false);
                        }
                    }

                    if (ignores != null && ignores.Length > 0)
                    {
                        foreach (string ignore in ignores)
                        {
                            fileInfoList.RemoveAll(a => a.FullName.Contains(ignore.FixSlash()) == true);
                        }
                    }
                    return fileInfoList.ToArray();
                }


            }
        }
    }
}