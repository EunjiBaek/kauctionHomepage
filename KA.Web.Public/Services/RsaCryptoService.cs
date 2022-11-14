using System;
using System.Security.Cryptography;
using System.Text;

namespace KA.Web.Public.Services
{
    /// <summary>
    /// RSA 암호화 서비스 클래스입니다.
    /// </summary>
    public interface IRsaCryptoService
    {
        /// <summary>
        /// 공개 키를 가져옵니다.
        /// </summary>
        /// <returns>공개 키</returns>
        public string GetPublicKey();

        /// <summary>
        /// 문자열을 암호화 합니다.
        /// </summary>
        /// <param name="text">텍스트</param>
        /// <returns>암호화된 문자열</returns>
        public string Encrypt(string text);

        /// <summary>
        /// 문자열을 복호화 합니다.
        /// </summary>
        /// <param name="encryptedText">암호화된 텍스트</param>
        /// <returns>복호화된 문자열</returns>
        /// <returns>복호화 실패시 String.Empty 반환</returns>
        public string Decrypt(string encryptedText);
    }
    
    /// <inheritdoc />
    public class RsaCryptoService : IRsaCryptoService
    {
        private readonly RSA _rsa;
        
        /// <summary>
        /// 생성자입니다.
        /// </summary>
        public RsaCryptoService()
        {
            //Generate a public/private key pair.  
            _rsa = RSA.Create();
        }
        
        /// <inheritdoc />
        public string GetPublicKey()
        {
            var publicKey = Convert.ToBase64String(_rsa.ExportSubjectPublicKeyInfo());
            var pem = $"-----BEGIN PUBLIC KEY-----\n{publicKey}\n-----END PUBLIC KEY-----";
            return pem;
        }

        /// <inheritdoc />
        public string Encrypt(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            var cipherText = _rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);

            return Convert.ToBase64String(cipherText);
        }
        
        /// <inheritdoc />
        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText)) throw new ArgumentNullException(nameof(encryptedText));
            
            var data = Convert.FromBase64String(encryptedText);
            string decrypted;
            try
            {
                var cipherText =_rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
                decrypted = Encoding.UTF8.GetString(cipherText);
            }
            catch (CryptographicException)
            {
                return string.Empty;
            }

            return decrypted;
        }
    }
}