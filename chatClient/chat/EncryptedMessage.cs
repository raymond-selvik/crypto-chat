namespace Cryptochat.Client
{
    public class EncryptedMessage
    {
        public byte[] EncryptedSessionKey;

        public byte[] Message;

        public byte[] IV;

        public byte[] Hmac;
        
        public byte[] Signature;
    }
}