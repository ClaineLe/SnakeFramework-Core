using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace com.snake.framework
{
    public static partial class Utility
    {
        /// <summary>
        /// 加密解密相关的实用函数。
        /// </summary>
        public static class Encryption
        {
            internal const int QuickEncryptLength = 220;

            /// <summary>
            /// 将 bytes 使用 code 做异或运算的快速版本。
            /// </summary>
            /// <param name="bytes">原始二进制流。</param>
            /// <param name="code">异或二进制流。</param>
            /// <returns>异或后的二进制流。</returns>
            public static byte[] GetQuickXorBytes(byte[] bytes, byte[] code)
            {
                return GetXorBytes(bytes, 0, QuickEncryptLength, code);
            }

            /// <summary>
            /// 将 bytes 使用 code 做异或运算的快速版本。此方法将复用并改写传入的 bytes 作为返回值，而不额外分配内存空间。
            /// </summary>
            /// <param name="bytes">原始及异或后的二进制流。</param>
            /// <param name="code">异或二进制流。</param>
            public static void GetQuickSelfXorBytes(byte[] bytes, byte[] code)
            {
                GetSelfXorBytes(bytes, 0, QuickEncryptLength, code);
            }

            /// <summary>
            /// 将 bytes 使用 code 做异或运算。
            /// </summary>
            /// <param name="bytes">原始二进制流。</param>
            /// <param name="code">异或二进制流。</param>
            /// <returns>异或后的二进制流。</returns>
            public static byte[] GetXorBytes(byte[] bytes, byte[] code)
            {
                if (bytes == null)
                {
                    return null;
                }

                return GetXorBytes(bytes, 0, bytes.Length, code);
            }

            /// <summary>
            /// 将 bytes 使用 code 做异或运算。此方法将复用并改写传入的 bytes 作为返回值，而不额外分配内存空间。
            /// </summary>
            /// <param name="bytes">原始及异或后的二进制流。</param>
            /// <param name="code">异或二进制流。</param>
            public static void GetSelfXorBytes(byte[] bytes, byte[] code)
            {
                if (bytes == null)
                {
                    return;
                }

                GetSelfXorBytes(bytes, 0, bytes.Length, code);
            }

            /// <summary>
            /// 将 bytes 使用 code 做异或运算。
            /// </summary>
            /// <param name="bytes">原始二进制流。</param>
            /// <param name="startIndex">异或计算的开始位置。</param>
            /// <param name="length">异或计算长度，若小于 0，则计算整个二进制流。</param>
            /// <param name="code">异或二进制流。</param>
            /// <returns>异或后的二进制流。</returns>
            public static byte[] GetXorBytes(byte[] bytes, int startIndex, int length, byte[] code)
            {
                if (bytes == null)
                {
                    return null;
                }

                int bytesLength = bytes.Length;
                byte[] results = new byte[bytesLength];
                Array.Copy(bytes, 0, results, 0, bytesLength);
                GetSelfXorBytes(results, startIndex, length, code);
                return results;
            }

            /// <summary>
            /// 将 bytes 使用 code 做异或运算。此方法将复用并改写传入的 bytes 作为返回值，而不额外分配内存空间。
            /// </summary>
            /// <param name="bytes">原始及异或后的二进制流。</param>
            /// <param name="startIndex">异或计算的开始位置。</param>
            /// <param name="length">异或计算长度。</param>
            /// <param name="code">异或二进制流。</param>
            public static void GetSelfXorBytes(byte[] bytes, int startIndex, int length, byte[] code)
            {
                if (bytes == null)
                {
                    return;
                }

                if (code == null)
                {
                    throw new Exception("Code is invalid.");
                }

                int codeLength = code.Length;
                if (codeLength <= 0)
                {
                    throw new Exception("Code length is invalid.");
                }

                if (startIndex < 0 || length < 0 || startIndex + length > bytes.Length)
                {
                    throw new Exception("Start index or length is invalid.");
                }

                int codeIndex = startIndex % codeLength;
                for (int i = startIndex; i < length; i++)
                {
                    bytes[i] ^= code[codeIndex++];
                    codeIndex %= codeLength;
                }
            }


            /// <summary>
            /// AES加密 
            /// </summary>
            /// <param name="text">加密字符</param>
            /// <param name="password">加密的密码</param>
            /// <param name="iv">密钥</param>
            /// <returns></returns>
            public static byte[] AESEncrypt(byte[] sourceBytes, string password, string iv)
            {
                var rijndaelCipher = new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 128,
                    BlockSize = 128
                };
                var pwdBytes = System.Text.Encoding.Default.GetBytes(password);
                var keyBytes = new byte[16];
                var len = pwdBytes.Length;
                if (len > keyBytes.Length) len = keyBytes.Length;
                System.Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                var ivBytes = System.Text.Encoding.Default.GetBytes(iv);
                rijndaelCipher.IV = ivBytes;
                var transform = rijndaelCipher.CreateEncryptor();
                var plainText = sourceBytes;
                var cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
                return cipherBytes;
            }

            /// <summary>
            /// AES解密
            /// </summary>
            /// <param name="text"></param>
            /// <param name="password"></param>
            /// <param name="iv"></param>
            /// <returns></returns>
            public static byte[] AESDecrypt(byte[] encryptedBytes, string password, string iv)
            {
                var rijndaelCipher = new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 128,
                    BlockSize = 128
                };
                var encryptedData = encryptedBytes;
                var pwdBytes = System.Text.Encoding.Default.GetBytes(password);
                var keyBytes = new byte[16];
                var len = pwdBytes.Length;
                if (len > keyBytes.Length) len = keyBytes.Length;
                System.Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                var ivBytes = System.Text.Encoding.Default.GetBytes(iv);
                rijndaelCipher.IV = ivBytes;
                var transform = rijndaelCipher.CreateDecryptor();
                var plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                return plainText;
            }

            /// <summary>
            /// 异或加密
            /// </summary>
            /// <param name="content"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static byte[] XOREncrypt(byte[] sourceByte, string password)
            {
                var encryptedContent = string.Empty;
                if (string.IsNullOrEmpty(password))
                {
                    Debug.LogError("传入密码为空,无法加密");
                    return sourceByte;
                }
                var sourceContentArray = sourceByte;
                var passwordArray = Encoding.Default.GetBytes(password);
                var passwordLength = passwordArray.Length;
                for (var i = 0; i < sourceContentArray.Length; i++)
                {
                    var p = passwordArray[i % passwordLength];
                    sourceContentArray[i] = (byte)(sourceContentArray[i] ^ p);
                }
                return sourceContentArray;
            }

            /// <summary>
            /// 异或解密
            /// </summary>
            /// <param name="content"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public static byte[] XORDecrypt(byte[] encryptedBytes, string password)
            {
                var sourceContent = string.Empty;
                if (string.IsNullOrEmpty(password))
                {
                    Debug.LogError("传入密码为空,无法解密");
                    return encryptedBytes;
                }
                var passwordArray = Encoding.Default.GetBytes(password);
                var passwordLength = passwordArray.Length;
                for (var i = 0; i < encryptedBytes.Length; i++)
                {
                    var p = passwordArray[i % passwordLength];
                    encryptedBytes[i] = (byte)(encryptedBytes[i] ^ p);
                }
                return encryptedBytes;
            }



            /****************************************************************** XXTEA *********************************************************************************************/

            private static readonly UTF8Encoding mXXTEAutf8 = new UTF8Encoding();

            private const UInt32 delta = 0x9E3779B9;

            private static UInt32 XXTEA_MX(UInt32 sum, UInt32 y, UInt32 z, Int32 p, UInt32 e, UInt32[] k)
            {
                return (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z);
            }

            public static Byte[] XXTEA_Encrypt(Byte[] data, Byte[] key)
            {
                if (data.Length == 0)
                {
                    return data;
                }
                return XXTEA_ToByteArray(XXTEA_Encrypt(XXTEA_ToUInt32Array(data, true), XXTEA_ToUInt32Array(XXTEA_FixKey(key), false)), false);
            }

            public static Byte[] XXTEA_Encrypt(String data, Byte[] key)
            {
                return XXTEA_Encrypt(mXXTEAutf8.GetBytes(data), key);
            }

            public static Byte[] XXTEA_Encrypt(Byte[] data, String key)
            {
                return XXTEA_Encrypt(data, mXXTEAutf8.GetBytes(key));
            }

            public static Byte[] XXTEA_Encrypt(String data, String key)
            {
                return XXTEA_Encrypt(mXXTEAutf8.GetBytes(data), mXXTEAutf8.GetBytes(key));
            }

            public static String XXTEA_EncryptToBase64String(Byte[] data, Byte[] key)
            {
                return Convert.ToBase64String(XXTEA_Encrypt(data, key));
            }

            public static String XXTEA_EncryptToBase64String(String data, Byte[] key)
            {
                return Convert.ToBase64String(XXTEA_Encrypt(data, key));
            }

            public static String XXTEA_EncryptToBase64String(Byte[] data, String key)
            {
                return Convert.ToBase64String(XXTEA_Encrypt(data, key));
            }

            public static String XXTEA_EncryptToBase64String(String data, String key)
            {
                return Convert.ToBase64String(XXTEA_Encrypt(data, key));
            }

            public static Byte[] XXTEA_Decrypt(Byte[] data, Byte[] key)
            {
                if (data.Length == 0)
                {
                    return data;
                }
                return XXTEA_ToByteArray(XXTEA_Decrypt(XXTEA_ToUInt32Array(data, false), XXTEA_ToUInt32Array(XXTEA_FixKey(key), false)), true);
            }

            public static Byte[] XXTEA_Decrypt(Byte[] data, String key)
            {
                return XXTEA_Decrypt(data, mXXTEAutf8.GetBytes(key));
            }

            public static Byte[] XXTEA_DecryptBase64String(String data, Byte[] key)
            {
                return XXTEA_Decrypt(Convert.FromBase64String(data), key);
            }

            public static Byte[] XXTEA_DecryptBase64String(String data, String key)
            {
                return XXTEA_Decrypt(Convert.FromBase64String(data), key);
            }

            public static String XXTEA_DecryptToString(Byte[] data, Byte[] key)
            {
                return mXXTEAutf8.GetString(XXTEA_Decrypt(data, key));
            }

            public static String XXTEA_DecryptToString(Byte[] data, String key)
            {
                return mXXTEAutf8.GetString(XXTEA_Decrypt(data, key));
            }

            public static String XXTEA_DecryptBase64StringToString(String data, Byte[] key)
            {
                return mXXTEAutf8.GetString(XXTEA_DecryptBase64String(data, key));
            }

            public static String XXTEA_DecryptBase64StringToString(String data, String key)
            {
                return mXXTEAutf8.GetString(XXTEA_DecryptBase64String(data, key));
            }

            private static UInt32[] XXTEA_Encrypt(UInt32[] v, UInt32[] k)
            {
                Int32 n = v.Length - 1;
                if (n < 1)
                {
                    return v;
                }
                UInt32 z = v[n], y, sum = 0, e;
                Int32 p, q = 6 + 52 / (n + 1);
                unchecked
                {
                    while (0 < q--)
                    {
                        sum += delta;
                        e = sum >> 2 & 3;
                        for (p = 0; p < n; p++)
                        {
                            y = v[p + 1];
                            z = v[p] += XXTEA_MX(sum, y, z, p, e, k);
                        }
                        y = v[0];
                        z = v[n] += XXTEA_MX(sum, y, z, p, e, k);
                    }
                }
                return v;
            }

            private static UInt32[] XXTEA_Decrypt(UInt32[] v, UInt32[] k)
            {
                Int32 n = v.Length - 1;
                if (n < 1)
                {
                    return v;
                }
                UInt32 z, y = v[0], sum, e;
                Int32 p, q = 6 + 52 / (n + 1);
                unchecked
                {
                    sum = (UInt32)(q * delta);
                    while (sum != 0)
                    {
                        e = sum >> 2 & 3;
                        for (p = n; p > 0; p--)
                        {
                            z = v[p - 1];
                            y = v[p] -= XXTEA_MX(sum, y, z, p, e, k);
                        }
                        z = v[n];
                        y = v[0] -= XXTEA_MX(sum, y, z, p, e, k);
                        sum -= delta;
                    }
                }
                return v;
            }

            private static Byte[] XXTEA_FixKey(Byte[] key)
            {
                if (key.Length == 16) return key;
                Byte[] fixedkey = new Byte[16];
                if (key.Length < 16)
                {
                    key.CopyTo(fixedkey, 0);
                }
                else
                {
                    Array.Copy(key, 0, fixedkey, 0, 16);
                }
                return fixedkey;
            }

            private static UInt32[] XXTEA_ToUInt32Array(Byte[] data, Boolean includeLength)
            {
                Int32 length = data.Length;
                Int32 n = (((length & 3) == 0) ? (length >> 2) : ((length >> 2) + 1));
                UInt32[] result;
                if (includeLength)
                {
                    result = new UInt32[n + 1];
                    result[n] = (UInt32)length;
                }
                else
                {
                    result = new UInt32[n];
                }
                for (Int32 i = 0; i < length; i++)
                {
                    result[i >> 2] |= (UInt32)data[i] << ((i & 3) << 3);
                }
                return result;
            }

            private static Byte[] XXTEA_ToByteArray(UInt32[] data, Boolean includeLength)
            {
                Int32 n = data.Length << 2;
                if (includeLength)
                {
                    Int32 m = (Int32)data[data.Length - 1];
                    n -= 4;
                    if ((m < n - 3) || (m > n))
                    {
                        return null;
                    }
                    n = m;
                }
                Byte[] result = new Byte[n];
                for (Int32 i = 0; i < n; i++)
                {
                    result[i] = (Byte)(data[i >> 2] >> ((i & 3) << 3));
                }
                return result;
            }
        }
    }
}