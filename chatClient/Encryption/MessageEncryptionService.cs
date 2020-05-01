using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json;

namespace Cryptochat.Client.Encryption
{
    public class MessageEncryptionService
    {
        private RsaKeys rsaKeys;

        public MessageEncryptionService()
        {
            var rsa = new RsaEncryption();
            this.rsaKeys = rsa.GenerateKeys();
        }

        public string EncryptMessage(string message, string publicKey)
        {
            var receiverPublicKey = Convert.FromBase64String(publicKey);
            var encryptedMessage = new EncryptedMessage();

            var aes = new AesEncryption(32, 16);
            encryptedMessage.IV = aes.iv;
            encryptedMessage.Message = aes.Encrypt(Encoding.UTF8.GetBytes(message));

            var sessionKey = aes.key;
            var rsa = new RsaEncryption();
            encryptedMessage.EncryptedSessionKey = rsa.Encrypt(sessionKey, receiverPublicKey);

            var hmac = new HmacAuthentication(sessionKey);
            encryptedMessage.Hmac = hmac.ComputeHash(encryptedMessage.Message);

            encryptedMessage.Signature = rsa.SignData(encryptedMessage.Hmac, rsaKeys.privateKey);

            return JsonConvert.SerializeObject(encryptedMessage);
        }

        public string DecryptMessage(string message, string publicKey)
        {
            var senderPublicKey = Convert.FromBase64String(publicKey);
            var encryptedMessage = JsonConvert.DeserializeObject<EncryptedMessage>(message);

            var rsa = new RsaEncryption();
            byte[] sessionKey = rsa.Decrypt(encryptedMessage.EncryptedSessionKey, rsaKeys.privateKey);

            var hmac = new HmacAuthentication(sessionKey);
            if(!hmac.VerifyHash(encryptedMessage.Hmac, encryptedMessage.Message))
            {
                throw new CryptographicException("HMAC does not match hash of message.");
            }

             if(!rsa.VerifySignature(encryptedMessage.Hmac, encryptedMessage.Signature, senderPublicKey))
            {
                throw new CryptographicException("Signature of message could not be verified.");
            }

            var aes = new AesEncryption(sessionKey, encryptedMessage.IV);

            var decryptedMessage = aes.Decrypt(encryptedMessage.Message);

            return Encoding.UTF8.GetString(decryptedMessage);
        }

        public string GetPublicKey()
        {
            return Convert.ToBase64String(this.rsaKeys.publicKey);
        }
    }
}