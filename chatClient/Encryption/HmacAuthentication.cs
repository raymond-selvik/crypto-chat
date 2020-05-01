using System.Security.Cryptography;

namespace Cryptochat.Client.Encryption
{
    public class HmacAuthentication
    {
        public byte[] key;

        public HmacAuthentication(byte[] key)
        {
            this.key = key;
        }

        public byte[] ComputeHash(byte[] data)
        {
            using(var hmac = new HMACSHA256(this.key))
            {
                return hmac.ComputeHash(data);
            }
        }

        public bool VerifyHash(byte[] hash, byte[] data)
        {
            using (var hmac = new HMACSHA256(key))
            {
                var calculatedHash = hmac.ComputeHash(data);

                return Compare(hash, calculatedHash);
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
    }
}