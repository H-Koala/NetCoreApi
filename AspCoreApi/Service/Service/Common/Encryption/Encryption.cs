using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Services.Common.Encryption
{
   public  class Encryption
    {
        public static string strKey = "hnj";
        public static string strIV = "jnh";
        #region 加密字符串  
        /// <summary> /// 加密字符串   
        /// </summary>  
        /// <param name="str">要加密的字符串</param>  
        /// <returns>加密后的字符串</returns>  
        public static string Encrypt(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();//实例化内存流对象
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider(); //实例化加/解密类对象
            //使用内存流实例化加密流对象 
            CryptoStream CStream = new CryptoStream(ms, descsp.CreateEncryptor(Encoding.UTF8.GetBytes(strKey), Encoding.UTF8.GetBytes(strIV)), CryptoStreamMode.Write);
            CStream.Write(buffer, 0, buffer.Length);
            CStream.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        #endregion
        #region 解密字符串   
        /// <summary>  
        /// 解密字符串   
        /// </summary>  
        /// <param name="str">要解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public static string Decrypt(string str)
        {
            byte[] buffer = Convert.FromBase64String(str);
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();
            CryptoStream CStream = new CryptoStream(ms, descsp.CreateDecryptor(Encoding.UTF8.GetBytes(strKey), Encoding.UTF8.GetBytes(strIV)), CryptoStreamMode.Write);
            CStream.Write(buffer, 0, buffer.Length);
            CStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
        #endregion
        public static byte[] DecryptBytes(string str)
        {
            byte[] buffer = Convert.FromBase64String(str);
            MemoryStream ms = new MemoryStream();
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();
            CryptoStream CStream = new CryptoStream(ms, descsp.CreateDecryptor(Encoding.UTF8.GetBytes(strKey), Encoding.UTF8.GetBytes(strIV)), CryptoStreamMode.Write);
            CStream.Write(buffer, 0, buffer.Length);
            CStream.FlushFinalBlock();
            return Encoding.UTF8.GetBytes(ms.ToString());
        }
    }
}
