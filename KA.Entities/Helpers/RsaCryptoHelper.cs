using System;
using System.Security.Cryptography;
using System.Text;

namespace KA.Entities.Helpers
{
    /// <summary>
    /// RSA 암호화 헬퍼 클래스입니다.(윈도우 OS Only)
    /// IIS에서 사용시 Application Pool의 Settings 중
    /// Advanced Settings > ProcessModel > Load User Profile 설정을 True로 활성화해야합니다.
    /// </summary>
    public static class RsaCryptoHelper
    {
        private const string KeyContainer = "K-AUCTION-CRYPTO-CONTAINER";

        /// <summary>
        /// 공개 키를 가져옵니다.
        /// </summary>
        /// <returns>공개 키</returns>
        public static string GetPublicKey()
        {
            var parameters = new CspParameters
            {
                KeyContainerName = KeyContainer
            };
            using var rsa = new RSACryptoServiceProvider(parameters);

            var publicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());
            var pem = $"-----BEGIN PUBLIC KEY-----\n{publicKey}\n-----END PUBLIC KEY-----";
            return pem;
        }

        /// <summary>
        /// 문자열을 암호화 합니다.
        /// </summary>
        /// <param name="text">텍스트</param>
        /// <returns>암호화된 문자열</returns>
        public static string Encrypt(string text)
        {
            var parameters = new CspParameters
            {
                KeyContainerName = KeyContainer
            };
            using var rsa = new RSACryptoServiceProvider(parameters);
            var data = Encoding.UTF8.GetBytes(text);
            var cipherText = rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);

            return Convert.ToBase64String(cipherText);
        }
        
        /// <summary>
        /// 문자열을 복호화 합니다.
        /// </summary>
        /// <param name="encryptedText">암호화된 텍스트</param>
        /// <returns>복호화된 문자열</returns>
        public static string Decrypt(string encryptedText)
        {
            var parameters = new CspParameters
            {
                KeyContainerName = KeyContainer
            };
            using var rsa = new RSACryptoServiceProvider(parameters);
            var data = Convert.FromBase64String(encryptedText);
            var cipherText = rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            
            return Encoding.UTF8.GetString(cipherText);
        }
    }
}