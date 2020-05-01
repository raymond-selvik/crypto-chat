using System.Security.Cryptography;

namespace Cryptochat.Client.Encryption
{
    public class RsaEncryption
    {
        public RsaKeys GenerateKeys()
        {
            using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;

                var publicKey = rsa.ExportRSAPublicKey();
                var privateKey = rsa.ExportRSAPrivateKey();

                return new RsaKeys(publicKey, privateKey);
            }
        }

        public byte[] Encrypt(byte[] data, byte[] publicKey)
        {
            using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportRSAPublicKey(publicKey, out _);

                return rsa.Encrypt(data, false);
            }
        }

        public byte[] Decrypt(byte[] encryptedData, byte[] privateKey)
        {
            using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportRSAPrivateKey(privateKey, out _);

                return rsa.Decrypt(encryptedData, false);
            }
        }

        public byte[] SignData(byte[] data, byte[] privateKey)
        {
             using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportRSAPrivateKey(privateKey, out _);

                var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
                rsaFormatter.SetHashAlgorithm("SHA256");

                return rsaFormatter.CreateSignature(data);
            }
        }

        public bool VerifySignature(byte[] data, byte[] signature, byte[] publicKey)
        {
            using(var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportRSAPublicKey(publicKey, out _);

                var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("SHA256");

                return rsaDeformatter.VerifySignature(data, signature);
            }
        }
    }
}