using System;
using Xunit;
using Stegosaurus.Crypto;


namespace Stegosaurus.Tests.Crypto
{
    public class Test_AesCryptoService
    {
        [Fact]
        public void EncryptDecrypt_ShouldReturnOriginalText()
        {
            string password = "stronkpassword123";
            string plaintext = "As you can see, my password is super stronk.";

            byte[] encryptedData = AesCryptoService.Encrypt(password, plaintext);
            string decryptedData = AesCryptoService.Decrypt(password, encryptedData);
            Assert.Equal(plaintext, decryptedData);
        }
    }
}