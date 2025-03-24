using Xunit;
using Stegosaurus.Stego;
using Stegosaurus.Crypto;
using SixLabors.ImageSharp;


namespace Stegosaurus.Tests.Integration
{
    public class Test_EndToEnd
    {
        [Fact]
        public void EndToEnd_EncryptEncodeDecodeDecrypt_WorksCorrectly()
        {
            string baseDir = AppContext.BaseDirectory;
            string password = "testpassword";
            string message = "Super duper secret test message";
            string inputImage = Path.Combine(baseDir, "Assets", "Images", "image_valid.png");  //"Assets/Images/image_valid.png";

            // Encrypt
            byte[] encrypted = AesCryptoService.Encrypt(password, message);

            // Encode
            var encoder = new LsbEncoder(inputImage, encrypted, password);
            var encodedImage = encoder.Encode();

            // Save to temp path
            string tempPath = Path.Combine(Path.GetTempPath(), $"encoded_{Guid.NewGuid()}.png");
            encodedImage.Save(tempPath);

            // Decode
            var decoder = new LsbDecoder(tempPath, password);
            byte[] decodedBytes = decoder.Decode();

            // Decrypt
            string finalMessage = AesCryptoService.Decrypt(password, decodedBytes);

            // Assert
            Assert.Equal(message, finalMessage);

            // Cleanup
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}