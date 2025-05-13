using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Encryption
{
    /// <summary>
    /// 加解密工具类
    /// </summary>
    public static class CryptoUtility
    {
        /// <summary>
        /// 加密密钥
        /// </summary>
        private const string _key = "A7$k8*Jq3#mC9@tB";

        /// <summary>
        /// 加密向量
        /// </summary>
        private const string _iv = "X5@r2*Lq7#F8$vY1";
        
        
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encrypt(string data)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.ASCII.GetBytes(_key);
            aes.IV = Encoding.ASCII.GetBytes(_iv);
            using MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(data);
                }
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Decrypt(string data)
        {
            var cipherBytes = Convert.FromBase64String(data);
            using Aes aes = Aes.Create();
            aes.Key = Encoding.ASCII.GetBytes(_key);
            aes.IV = Encoding.ASCII.GetBytes(_iv);
            using MemoryStream ms = new MemoryStream(cipherBytes);
            using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }


        /// <summary>
        /// 判断是否过期
        /// </summary>
        /// <param name="expirationDate"></param>
        /// <returns></returns>
        public static bool IsExpired(string expirationDate)
        {
            if (string.IsNullOrEmpty(expirationDate))
            {
                return false;
            }

            if (expirationDate.Equals("Never"))
            {
                return false;
            }

            DateTime expiration = DateTime.ParseExact(expirationDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
            return DateTime.Now > expiration;
        }
    }
}