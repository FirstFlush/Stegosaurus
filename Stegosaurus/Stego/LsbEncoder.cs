using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace Stegosaurus.Stego
{
    public class LsbEncoder
    {
        private readonly Image<Rgba32> _image;
        private readonly byte[] _ciphertext;
        private readonly Random _prng;

        public LsbEncoder(string filePath, byte[] ciphertext, string password)
        {
            _ciphertext = ciphertext;
            _prng = StegoConstants.Prng(password);
            _image = Image.Load<Rgba32>(filePath);
        }

        private byte[] PrefixCiphertext()
        {
            var prefix = BitConverter.GetBytes(_ciphertext.Length); // _ciphertext.Length will return int32, so 4 bytes by default
            if (BitConverter.IsLittleEndian)
                Array.Reverse(prefix);
            
            if (prefix.Length != StegoConstants.PrefixLength)
                throw new InvalidOperationException("Unexpected prefix size"); // 4-byte prefix can accommodate a message of over 2 billion chars lol

            byte[] prefixedCiphertext = new byte[prefix.Length + _ciphertext.Length];
            Buffer.BlockCopy(prefix, 0, prefixedCiphertext, 0, prefix.Length);
            Buffer.BlockCopy(_ciphertext, 0, prefixedCiphertext, prefix.Length, _ciphertext.Length);
            return prefixedCiphertext;
        }

        private void CheckCapacity()
        {
            var capacityBytes = _image.Width * _image.Height * 3 / 8;
            if (_ciphertext.Length + StegoConstants.PrefixLength > capacityBytes)
                throw new InvalidOperationException("Image is not large enough to encode message of this length");
        }

        private byte SetLsb(byte value, int bit)
        {
            return (byte)((value & 0b11111110) | (bit & 1));
        }

        public Image<Rgba32> Encode()
        {
            CheckCapacity();
            const int maxRetries = 10000;
            var payload = PrefixCiphertext();
            var clonedImage = _image.Clone();
            int totalChannels = clonedImage.Width * clonedImage.Height * 3; // * 3 because we manipulate R,G,B channels and ignore A. Manipulating A channel can cause image distortion.
            var usedChannels = new HashSet<int>(); // this HashSet makes sure we don't overwrite a previously-encoded bit if our PRNG produces the same number twice.

            for (int i = 0; i < payload.Length; i++)
            {
                byte b = payload[i];
                for (int bitIndex = 7; bitIndex >= 0; bitIndex--)
                {
                    int bit = (b >> bitIndex) & 1;
                    int channelIndex;
                    int retries = 0;

                    do
                    {
                        channelIndex = _prng.Next(totalChannels);
                        retries++;
                        if (retries > maxRetries)
                            throw new TimeoutException("Too many attempts to find unused channel. Image may be full.");
                    } while (!usedChannels.Add(channelIndex));

                    int pixelIndex = channelIndex / 3;
                    int channelOffset = channelIndex % 3;

                    int x = pixelIndex % clonedImage.Width;
                    int y = pixelIndex / clonedImage.Width;

                    var pixel = clonedImage[x, y];

                    switch (channelOffset)
                    {
                        case 0: pixel.R = SetLsb(pixel.R, bit); break;
                        case 1: pixel.G = SetLsb(pixel.G, bit); break;
                        case 2: pixel.B = SetLsb(pixel.B, bit); break;
                    }
                    clonedImage[x, y] = pixel;
                }
            }
            return clonedImage;
        }
    }
}