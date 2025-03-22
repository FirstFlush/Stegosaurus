using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace Stegosaurus.Stego
{
    public class LsbEncoder
    {
        private readonly Image<Rgba32> _image;
        private readonly byte[] _ciphertext;

        public LsbEncoder(string filePath, byte[] ciphertext)
        {
            _image = Image.Load<Rgba32>(filePath);
            _ciphertext = ciphertext;
        }

        // private void CheckCapacity()
        // {
        //     var capacityBytes = _image.Width * _image.Height * 3 / 8;
        //     if (_ciphertext.Length + StegoConstants.PrefixLength > capacityBytes)
        //         throw new InvalidOperationException("Image is not large enough to encode message of this length");
        // }

        public Image<Rgba32> Encode()
        {
            // CheckCapacity();
            var clonedImage = _image.Clone();


            return clonedImage;
        }


    }
}