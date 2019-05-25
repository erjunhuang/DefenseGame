

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
namespace QGame.Utils
{
    class HashUtils
    {

        /// <summary>
        /// 取字符串MD5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5(string str)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
