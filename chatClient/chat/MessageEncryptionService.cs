using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cryptochat.Client
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

        public EncryptedMessage EncryptMessage(string message, byte[] receiverPublicKey)
        {
            var encryptedMessage = new EncryptedMessage();

            var sessionKey = GenerateRandomNumber(32);

            var iv = GenerateRandomNumber(16);
            encryptedMessage.IV = iv;

            encryptedMessage.Message = EncryptMessagePayload(message, sessionKey, iv);
            encryptedMessage.EncryptedSessionKey = EncryptSessionKey(sessionKey, receiverPublicKey);
            encryptedMessage.Hmac = ComputeMessageHash(sessionKey, encryptedMessage.Message);
            encryptedMessage.Signature = SignMessage(encryptedMessage.Hmac);

            return encryptedMessage;
        }

        public string DecryptMessage(EncryptedMessage encryptedMessage, byte[] receiverPublicKey)
        {
            byte[] sessionKey = DecryptSessionKey(encryptedMessage.EncryptedSessionKey);

            VerifyHash(sessionKey, encryptedMessage.Hmac, encryptedMessage.Message);
            VerifySignature(encryptedMessage.Hmac, encryptedMessage.Signature, receiverPublicKey);

            string message = DecryptMessagePayload(encryptedMessage.Message, sessionKey, encryptedMessage.IV);

            return message;
        }

        byte[] EncryptMessagePayload(string messageString, byte[] sessionKey, byte[] iv)
        {
            byte[] message = Encoding.UTF8.GetBytes(messageString);

            using(var aes = new AesCryptoServiceProvider())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = sessionKey; 
                aes.IV = iv;

                using (var memorystream = new MemoryStream())
                {
                    var cryptostream = new CryptoStream(memorystream, aes.CreateEncryptor(), CryptoStreamMode.Write);

                    cryptostream.Write(message, 0, message.Length);
                    cryptostream.FlushFinalBlock();

                    return memorystream.ToArray();
                }
            }
        }

        string DecryptMessagePayload(byte[] encryptedMessage, byte[] sessionKey, byte[] iv)
        {
            using (var des = new AesCryptoServiceProvider())
            {
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                des.Key = sessionKey;
                des.IV = iv;

                using (var memorystream = new MemoryStream())
                {
                    var cryptostream = new CryptoStream(memorystream, des.CreateDecryptor(), CryptoStreamMode.Write);

                    cryptostream.Write(encryptedMessage, 0, encryptedMessage.Length);
                    cryptostream.FlushFinalBlock();

                    return Encoding.UTF8.GetString(memorystream.ToArray());
                }
            }
        }

        byte[] EncryptSessionKey(byte[] sessionKey, byte[] receiverPublicKey)
        {
            using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportRSAPublicKey(receiverPublicKey, out _);

                return rsa.Encrypt(sessionKey, true);
            }
        }

        byte[] DecryptSessionKey(byte[] encryptedSessionKey)
        {
            using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportRSAPublicKey(this.publicKey, out _);
                rsa.ImportRSAPrivateKey(this.privateKey, out _);

                return rsa.Decrypt(encryptedSessionKey, true);
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
            using(var rsa = new RSACryptoServiceProvider())
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


        byte[] GenerateRandomNumber(int length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var num = new byte[length];

                rng.GetBytes(num);

                return num;
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