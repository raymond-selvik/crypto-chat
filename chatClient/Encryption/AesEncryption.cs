using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cryptochat.Client.Encryption
{
    public class AesEncryption
    {
        public byte[] key {get; private set;}
        public byte[] iv {get; private set;}


        public AesEncryption(int keyLength, int ivLength)
        {
            key = GenerateRandomNumber(keyLength);
            iv = GenerateRandomNumber(ivLength);
        }


        public AesEncryption(byte[] key, byte[] iv)
        {
            this.key = key;
            this.iv = iv;
        }

        public byte[] Encrypt(string data)
        {
            byte[] message = Encoding.UTF8.GetBytes(data);

            using(var aes = new AesCryptoServiceProvider())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = this.key; 
                aes.IV = this.iv;

                using (var memorystream = new MemoryStream())
                {
                    var cryptostream = new CryptoStream(memorystream, aes.CreateEncryptor(), CryptoStreamMode.Write);

                    cryptostream.Write(message, 0, message.Length);
                    cryptostream.FlushFinalBlock();

                    return memorystream.ToArray();
                }
            }
        }

        public string Decrypt(byte[] encryptedMessage)
        {
            using (var des = new AesCryptoServiceProvider())
            {
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                des.Key = this.key;
                des.IV = this.iv;

                using (var memorystream = new MemoryStream())
                {
                    var cryptostream = new CryptoStream(memorystream, des.CreateDecryptor(), CryptoStreamMode.Write);

                    cryptostream.Write(encryptedMessage, 0, encryptedMessage.Length);
                    cryptostream.FlushFinalBlock();

                    return Encoding.UTF8.GetString(memorystream.ToArray());
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
    }

}