using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace com.halo.framework
{
    public class AddressableFileSystem
    {
        private string _path;

        private FileStream _fileStream;
        private BinaryReader _binReader;
        private BinaryWriter _binWriter;
        private Dictionary<string, AddressableClip> _addressableClipDic;

        private Dictionary<string, AddressableClip> _cacheClipDic;
        private AddressableFileSystem() { }
        static public AddressableFileSystem Create(string path, FileMode fileMode, FileAccess fileAccess)
        {
            AddressableFileSystem afs = new AddressableFileSystem();
            afs._addressableClipDic = new Dictionary<string, AddressableClip>();
            afs._path = path;
            afs._fileStream = new FileStream(path, fileMode, fileAccess);
            afs._binWriter = new BinaryWriter(afs._fileStream);
            afs._binReader = new BinaryReader(afs._fileStream);
            afs._addressableClipDic = AddressableFileSystem.LoadAddressableClips(afs._binReader);
            return afs;
        }

        static public Dictionary<string, AddressableClip> LoadAddressableClips(BinaryReader binaryReader)
        {

            Dictionary<string, AddressableClip> addressableClipDic = new Dictionary<string, AddressableClip>();
            binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
            long streamLength = binaryReader.BaseStream.Length;

            try
            {
                while (binaryReader.BaseStream.Position < streamLength)
                {
                    AddressableClip fileClip = new AddressableClip();
                    fileClip.address = binaryReader.ReadString();

                    fileClip.offset = binaryReader.ReadInt64();
                    fileClip.obsolete = binaryReader.ReadBoolean();

                    if (fileClip.obsolete == false)
                    {
                        //废弃的丢弃
                        fileClip.length = binaryReader.ReadInt32();
                        addressableClipDic.Add(fileClip.address, fileClip);
                    }
                    else
                    {
                        fileClip.length = binaryReader.ReadInt32();
                    }

                    binaryReader.BaseStream.Position += fileClip.length;
                }
            }

            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

            return addressableClipDic;
        }

        public Dictionary<string, AddressableClip>.KeyCollection.Enumerator GetAddressKeyEnumerator()
        {
            return _addressableClipDic.Keys.GetEnumerator();
        }

        public void Write(string address, byte[] datas)
        {
            this._fileStream.Seek(0, SeekOrigin.End);
            this._binWriter.Write(address);
            //currPosition + offset(long,8bit) + obsolete(bool,1bit) + dataLen(int,4bit)
            this._binWriter.Write(this._fileStream.Position + 8 + 1 + 4);
            this._binWriter.Write(false);
            this._binWriter.Write(datas.Length);//数据长度
            this._binWriter.Write(datas);
        }

        public bool tryRead(string address, ref byte[] datas)
        {
            AddressableClip addressableClip;
            if (this._addressableClipDic.TryGetValue(address, out addressableClip) == false)
            {
                return false;
            }

            datas = new byte[addressableClip.length];
            this._binReader.BaseStream.Seek(addressableClip.offset, SeekOrigin.Begin);
            this._binReader.BaseStream.Read(datas, 0, datas.Length);

            return true;
        }


        private void ObsoleteFile(AddressableClip clip)
        {
            clip.obsolete = true;
            //offset = file offset - obsolete(bool,1bit) - dataLen(long,4bit)

            long offset = clip.offset - 1 - 4;
            this._fileStream.Seek(offset, SeekOrigin.Begin);
            this._binWriter.Write(true);


        }

        public void MergeWrite(string address, byte[] datas, bool isFlush = true)
        {



            if (isFlush)
            {
                AddressableClip addressableClip;
                if (this._addressableClipDic.TryGetValue(address, out addressableClip) == true)
                {
                    ObsoleteFile(addressableClip);
                }
                else
                {
                    addressableClip = new AddressableClip();
                    _addressableClipDic.Add(address, addressableClip);
                }
                this._fileStream.Seek(0, SeekOrigin.End);
                this._binWriter.Write(address);
                //currPosition + offset(long,8bit) + obsolete(bool,1bit) + dataLen(long,8bit)
                long offset = this._fileStream.Position + 8 + 1 + 4;

                this._binWriter.Write(offset);
                this._binWriter.Write(false);
                this._binWriter.Write(datas.Length);//数据长度
                this._binWriter.Write(datas);

                addressableClip.address = address;
                addressableClip.offset = offset;
                addressableClip.obsolete = false;

                addressableClip.length = datas.Length;

                addressableClip.content = datas;
            }
            else
            {
                _cacheClipDic.Add(address, new AddressableClip()
                {
                    address = address,
                    content = datas,
                });
            }

        }

        public void Flush()
        {
            if (_cacheClipDic != null)
            {
                foreach (var cache in _cacheClipDic)
                {
                    AddressableClip newClip = cache.Value;
                    AddressableClip addressableClip;
                    byte[] datas = newClip.content;
                    if (this._addressableClipDic.TryGetValue(newClip.address, out addressableClip) == true)
                    {
                        ObsoleteFile(addressableClip);
                    }
                    else
                    {
                        addressableClip = new AddressableClip();
                        _addressableClipDic.Add(newClip.address, addressableClip);
                    }
                    this._fileStream.Seek(0, SeekOrigin.End);
                    this._binWriter.Write(newClip.address);
                    //currPosition + offset(long,8bit) + obsolete(bool,1bit) + dataLen(long,8bit)
                    long offset = this._fileStream.Position + 8 + 1 + 4;
                    this._binWriter.Write(offset);
                    this._binWriter.Write(false);
                    this._binWriter.Write(datas.Length);//数据长度
                    this._binWriter.Write(datas);

                    addressableClip.address = newClip.address;
                    addressableClip.offset = offset;
                    addressableClip.obsolete = false;

                    addressableClip.length = datas.Length;
                }

                _cacheClipDic.Clear();
            }

        }

        public void Release()
        {
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream.Dispose();
                _fileStream = null;
            }
        }
    }
}