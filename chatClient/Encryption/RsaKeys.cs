namespace Cryptochat.Client.Encryption
{
    public struct RsaKeys
    {
        public byte[] publicKey {get;}
        public byte[] privateKey {get;}

        public RsaKeys(byte[] publicKey, byte[] privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
        }
    }
}