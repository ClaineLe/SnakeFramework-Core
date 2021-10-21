using System.IO;

namespace com.snake.framework
{
    namespace runtime
    {

        public class AddressableFileSystemHelper
        {
            static public void MergeAssetBundleWithAddressableFileSystem(string fromFoldFullPath, string toFileFullPath)
            {
                if (System.IO.File.Exists(toFileFullPath) == true)
                    System.IO.File.Delete(toFileFullPath);
                AddressableFileSystem afs = AddressableFileSystem.Create(toFileFullPath, System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite);

                if (System.IO.Directory.Exists(fromFoldFullPath) == false)
                {
                    throw new System.Exception("[MergeAssetBundleWithAddressableFileSystem]传入目录不存在.fromFold:" + fromFoldFullPath);
                }

                string[] files = System.IO.Directory.GetFiles(fromFoldFullPath, "*.*", System.IO.SearchOption.AllDirectories);
                string tmpAddress;
                string parent = fromFoldFullPath.Replace("\\", "/");
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].EndsWith(".meta") || files[i].Contains(".idea") || files[i].Contains(".vscode") || files[i].Contains(".DS_Store"))
                        continue;
                    tmpAddress = files[i].Replace("\\", "/");

                    tmpAddress = tmpAddress.Replace(parent + "/", "");
                    afs.Write(tmpAddress, System.IO.File.ReadAllBytes(files[i]));
                    //Debug.Log("tmpAddress:" + tmpAddress);
                }
                afs.Release();
            }
            static public void SplitAssetBundleWithAddressableFileSystem(string fromFileFullPath, string toFoldFullPath)
            {
                if (System.IO.File.Exists(fromFileFullPath) == false)
                    throw new System.Exception("[SplitAssetBundleWithAddressableFileSystem]传入文件不存在.fromFileFullPath:" + fromFileFullPath);

                if (System.IO.Directory.Exists(toFoldFullPath) == true)
                    System.IO.Directory.Delete(toFoldFullPath, true);
                System.IO.Directory.CreateDirectory(toFoldFullPath);

                AddressableFileSystem afs = AddressableFileSystem.Create(fromFileFullPath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);
                byte[] datas = null;
                using (var keyEnumerator = afs.GetAddressKeyEnumerator())
                {
                    string address = string.Empty;
                    while (keyEnumerator.MoveNext())
                    {
                        address = keyEnumerator.Current;
                        if (afs.tryRead(address, ref datas) == false)
                        {
                            throw new System.Exception("[SplitAssetBundleWithAddressableFileSystem]寻址错误.fromFileFullPath:" + fromFileFullPath);
                        }
                        string dirPath = Path.GetDirectoryName(toFoldFullPath + "/" + address);
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        System.IO.File.WriteAllBytes(toFoldFullPath + "/" + address, datas);
                    }
                }
                afs.Release();
            }
            static public byte[] ReadFile(AddressableFileSystem afs, string path)
            {

                byte[] datas = null;
                if (afs.tryRead(path, ref datas) == false)
                {
                    throw new System.Exception("[SplitAssetBundleWithAddressableFileSystem]寻址错误.fromFileFullPath:" + path);
                }
                return datas;
            }


            static public void AppendFile(AddressableFileSystem afs, string fromFoldFullPath, bool isFlush = true)
            {

                if (System.IO.Directory.Exists(fromFoldFullPath) == false)
                {
                    throw new System.Exception("[MergeAssetBundleWithAddressableFileSystem]传入目录不存在.fromFold:" + fromFoldFullPath);
                }

                string[] files = System.IO.Directory.GetFiles(fromFoldFullPath, "*.*", System.IO.SearchOption.AllDirectories);
                fromFoldFullPath = fromFoldFullPath.Replace("\\", "/") + "/";

                string tmpAddress;
                for (int i = 0; i < files.Length; i++)
                {
                    tmpAddress = files[i].Replace("\\", "/");
                    tmpAddress = tmpAddress.Replace(fromFoldFullPath, string.Empty);
                    byte[] datas = System.IO.File.ReadAllBytes(files[i]);
                    afs.MergeWrite(tmpAddress, datas, isFlush);
                }
            }
        }
    }
}