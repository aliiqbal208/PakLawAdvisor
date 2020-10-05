using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using System.Text;
using System.Security.Cryptography;

namespace appointments365.Controllers
{
    public class EncDec
    {
        private byte[] key = { };
        private byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
        //public static string s;
        public EncDec()
        {
        }
        public static string Encrypt(string stringToEncrypt, string sEncryptionKey)
        {
            string r = "";
            byte[] key = { };
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray; //Convert.ToByte(stringToEncrypt.Length)
            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                r = Convert.ToBase64String(ms.ToArray());
            }
            catch
            {

            }
            return r;
        }
        public string enc(string strQueryString)
        {
            return Encrypt(strQueryString, "!#$a54?3");
        }
        public static string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {
            string r = "";
            byte[] key = { };
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray = new byte[stringToDecrypt.Length + 1];
            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt.Replace(" ", "+"));
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                r = encoding.GetString(ms.ToArray());
            }
            catch
            {

            }
            return r;
        }
        public string dec(string strQueryString)
        {
            return Decrypt(strQueryString, "!#$a54?3");
        }
    }
}