using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KA.Entities.Helpers
{
    public static class DESCryptoHelper
    {
        private const string DESKey = "a2FeX2tleSQ=";
        private const string DESIV = "a2FeX19pdiQ=";

        /// <summary> 입력받은 데이터를 DESCryptoService를 사용하여 암호화한다. </summary>
        /// <param name="arr">암호화할 byte[]</param>
        /// <returns>암호화된 String</returns>
        public static string DESEncrypt(byte[] arr)
        {
            try
            {
                return DESEncrypt(Encoding.Default.GetString(arr));
            }
            catch (Exception) { return string.Empty; }
        }

        /// <summary> 입력받은 데이터를 DESCryptoService를 사용하여 암호화한다. </summary>
        /// <param name="str">암호화할 string</param>
        /// <returns>암호화된 String</returns>
        public static string DESEncrypt(string str)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(Convert.FromBase64String(DESKey), Convert.FromBase64String(DESIV)), CryptoStreamMode.Write);

                StreamWriter sw = new StreamWriter(cs);

                sw.WriteLine(str);

                sw.Close();
                cs.Close();

                string enc_str = Convert.ToBase64String(ms.ToArray());

                ms.Close();

                return enc_str;
            }
            catch (Exception) { return str; }
        }

        /// <summary> 입력받은 데이터를 DESCryptoService를 사용하여 복호화한다. </summary>
        /// <param name="arr">복호화할 byte[]</param>
        /// <returns>복호화된 String</returns>
        public static string DESDecrypt(byte[] arr)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                MemoryStream ms = new MemoryStream(arr);
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(Convert.FromBase64String(DESKey), Convert.FromBase64String(DESIV)), CryptoStreamMode.Read);

                StreamReader sr = new StreamReader(cs);

                string dec_str = sr.ReadLine();

                sr.Close();
                cs.Close();
                ms.Close();

                return dec_str;
            }
            catch (Exception) { return string.Empty; }
        }

        /// <summary> 입력받은 데이터를 DESCryptoService를 사용하여 복호화한다. </summary>
        /// <param name="str">복호화할 string</param>
        /// <returns>복호화된 String</returns>
        public static string DESDecrypt(string str)
        {
            try
            {
                return DESDecrypt(Convert.FromBase64String(str));
            }
            catch (Exception) { return string.Empty; }
        }
    }
}
