using Xunit;
using Cryptochat.Client.Encryption;
using System.Text;

public class TestClass
{
    string testMessage = "Test Message";

    [Fact]
    public void RsaEncrpytionTest()
    {
        var rsa = new RsaEncryption();
        RsaKeys keys = rsa.GenerateKeys();

        var data = Encoding.UTF8.GetBytes(testMessage);

        var encryptedData = rsa.Encrypt(data ,keys.publicKey);
        var decryptedData = rsa.Decrypt(encryptedData, keys.privateKey);

        Assert.Equal(data, decryptedData);
    }

    [Fact]
    public void AesEncryptionTest()
    {
    
        var aes = new AesEncryption(32, 16);

        var data = Encoding.UTF8.GetBytes(testMessage);

        var encryptedData = aes.Encrypt(data);
        var decryptedData = aes.Decrypt(encryptedData);

        Assert.Equal(data, decryptedData);
    }

    [Fact]
    public void RsaSigningTest()
    {
        var rsa = new RsaEncryption();
        RsaKeys keys = rsa.GenerateKeys();

        var hmacKey = Encoding.UTF8.GetBytes("HMAC KEY");
        var hmac = new HmacAuthentication(hmacKey);

        var data = Encoding.UTF8.GetBytes(testMessage);
        var hash = hmac.ComputeHash(data);

        var signature = rsa.SignData(hash, keys.privateKey);
        var isValid = rsa.VerifySignature(hash, signature, keys.publicKey);
        
        Assert.True(isValid);
    }

    [Fact]
    public void HmacVerficationTest()
    {
        var key = Encoding.UTF8.GetBytes("HMAC KEY");
        var data = Encoding.UTF8.GetBytes(testMessage);

        var hmac = new HmacAuthentication(key);
        var hash = hmac.ComputeHash(data);

        var isValid = hmac.VerifyHash(hash, data);

        Assert.True(isValid);
    }
}