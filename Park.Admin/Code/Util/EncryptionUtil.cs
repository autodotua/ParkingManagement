using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Park.Admin
{
    /// <summary>
    /// 加密字符串类
    /// </summary>
    public class EncryptionUtil
    {

        /// <summary>
        /// 创建字符串的MD5哈希值
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns>字符串MD5哈希值的十六进制字符串</returns>
        public static string StringToMD5Hash(string inputString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }
    }
}
