using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace com.snake.framework
{
    namespace runtime
    {
        public static partial class Utility
        {
            /// <summary>
            /// 校验文件MD5
            /// </summary>
            /// <param name="fileName"></param>
            /// <param name="preMD5"></param>
            /// <returns></returns>
            static public bool CheckFileMD5(string fileName, string preMD5)
            {
                int bit = preMD5.Length;
                if (bit != 16 && bit != 32)
                {
                    SnakeDebuger.ErrorFormat("[文件MD5校验错误]md5格式错误，长度应为16位或32位.\nfileName:{0}\npreMD5:{1}", fileName, preMD5);
                    return false;
                }

                string md5 = FileMD5(fileName);
                if (string.IsNullOrEmpty(md5) == true)
                {
                    SnakeDebuger.ErrorFormat("[文件MD5校验错误]获取md5失败，长度应为16位或32位.\nfileName:{0}\npreMD5:{1}", fileName, preMD5);
                    return false;
                }

                return md5.Equals(preMD5);
            }

            /// <summary>
            /// 获取文件的MD5码
            /// </summary>
            /// <param name="fileName">传入的文件名（含路径及后缀名）</param>
            /// <returns></returns>
            static public string FileMD5(string fileName, int bit = 32)
            {
                try
                {
                    FileStream file = new FileStream(fileName, FileMode.Open);
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(file);
                    file.Close();

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }

                    if (bit == 16)
                        return sb.ToString().Substring(8, 16);
                    else
                    if (bit == 32)
                        return sb.ToString();//默认情况
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
                }
            }
            static public string StringMD5(string str, int bit = 16)
            {
                return ByteMD5(Encoding.UTF8.GetBytes(str), bit);
            }
            static public string ByteMD5(byte[] str, int bit = 16)
            {
                MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
                byte[] hashedDataBytes;
                hashedDataBytes = md5Hasher.ComputeHash(str);
                StringBuilder tmp = new StringBuilder();
                foreach (byte i in hashedDataBytes)
                {
                    tmp.Append(i.ToString("x2"));
                }
                if (bit == 16)
                    return tmp.ToString().Substring(8, 16);
                else
                if (bit == 32) return tmp.ToString();//默认情况
                else return string.Empty;
            }
        }
    }
}