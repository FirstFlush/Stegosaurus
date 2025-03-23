using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace Stegosaurus.Stego
{
    public class LsbDecoder
    {
        private readonly Image<Rgba32> _image;
        private readonly Random _prng;

        public LsbDecoder(string filePath, string password)
        {
            _prng = StegoConstants.Prng(password);
            _image = Image.Load<Rgba32>(filePath);
        }
    }
}