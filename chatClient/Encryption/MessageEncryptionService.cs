using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json;

namespace Cryptochat.Client.Encryption
{
    public class MessageEncryptionService
    {
        private byte[] publicKey;
        private byte[] privateKey;

        public MessageEncryptionService()
        {
            using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;

                this.publicKey = rsa.ExportRSAPublicKey();
                this.privateKey = rsa.ExportRSAPrivateKey();
            }
        }

        public string EncryptMessage(string message, byte[] receiverPublicKey)
        {
            var encryptedMessage = new EncryptedMessage();

            var aes = new AesEncryption(32, 16);
            encryptedMessage.IV = aes.iv;
            encryptedMessage.Message = aes.Encrypt(message);

            var sessionKey = aes.key;

            encryptedMessage.EncryptedSessionKey = EncryptSessionKey(sessionKey, receiverPublicKey);
            encryptedMessage.Hmac = ComputeMessageHash(sessionKey, encryptedMessage.Message);
            encryptedMessage.Signature = SignMessage(encryptedMessage.Hmac);

            var serializedMessage = JsonConvert.SerializeObject(encryptedMessage);

            return serializedMessage;
        }

        public string DecryptMessage(EncryptedMessage encryptedMessage, byte[] receiverPublicKey)
        {
            Console.WriteLine("Decrpyting.....");
            byte[] sessionKey = DecryptSessionKey(encryptedMessage.EncryptedSessionKey);
            Console.WriteLine(Convert.ToBase64String(sessionKey));

            try
            {
                VerifyHash(sessionKey, encryptedMessage.Hmac, encryptedMessage.Message);
            }
            catch(CryptographicException e) 
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                VerifySignature(encryptedMessage.Hmac, encryptedMessage.Signature, receiverPublicKey);
            }
            catch(CryptographicException e) 
            {
                Console.WriteLine(e.Message);
            }

            var aes = new AesEncryption(sessionKey, encryptedMessage.IV);
            string message = aes.Decrypt(encryptedMessage.Message);

            return message;
        }

        byte[] EncryptSessionKey(byte[] sessionKey, byte[] receiverPublicKey)
        {
            using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportRSAPublicKey(receiverPublicKey, out _);

                return rsa.Encrypt(sessionKey, false);
            }
        }

        byte[] DecryptSessionKey(byte[] encryptedSessionKey)
        {
            using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportRSAPublicKey(this.publicKey, out _);
                rsa.ImportRSAPrivateKey(this.privateKey, out _);

                return rsa.Decrypt(encryptedSessionKey, false);
            }
        }

        byte[] ComputeMessageHash(byte[] sessionKey, byte[] encryptedMessage)
        {
            using(var hmac = new HMACSHA256(sessionKey))
            {
                return hmac.ComputeHash(encryptedMessage);
            }
        }

        void VerifyHash(byte[] sessionKey, byte[] hash, byte[] encryptedMessage)
        {
            using (var hmac = new HMACSHA256(sessionKey))
            {
                var calculatedHash = hmac.ComputeHash(encryptedMessage);

                if(!Compare(hash, calculatedHash))
                {
                    throw new CryptographicException("HMAC does not match encrpyted message.");
                }
            }
        }

        byte[] SignMessage(byte[] hmac)
        {
            using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportRSAPrivateKey(this.privateKey, out _);

                var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
                rsaFormatter.SetHashAlgorithm("SHA256");

                return rsaFormatter.CreateSignature(hmac);
            }

        }

        void VerifySignature(byte[] hash, byte[] signature, byte[] receiverPublicKey)
        {
            using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportRSAPublicKey(receiverPublicKey, out _);

                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("SHA256");

                if(!rsaDeformatter.VerifySignature(hash, signature))
                {
                    throw new CryptographicException("Signature cannnot be verified.");
                }
            }
        }


        bool Compare(byte[] array1, byte[] array2)
        {
            var result = array1.Length == array2.Length;

            for(var i = 0; i < array1.Length && i < array2.Length; ++i)
            {
                result &= array1[i]==array2[i];
            }

            return result;
        }

        public byte[] GetPublicKey()
        {
            return this.publicKey;
        }
    }

}