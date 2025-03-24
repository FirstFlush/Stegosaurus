using Xunit;
using Stegosaurus.Stego;
using SixLabors.ImageSharp;

namespace Stegosaurus.Tests.Integration
{
    public class Test_InvalidImage
    {

        [Fact]
        public void LsbEncoder_ShouldThrow_WhenImageIsInvalid()
        {
            // Arrange
            string baseDir = AppContext.BaseDirectory;
            string inputImage = Path.Combine(baseDir, "Assets", "Images", "image_invalid.png");
            string password = "testpassword";
            byte[] fakeCiphertext = [0x01, 0x02, 0x03];

            // Act & Assert
            Assert.Throws<UnknownImageFormatException>(() => new LsbEncoder(inputImage, fakeCiphertext, password));
        }


    }
}
