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

        public byte[] Decode()
        {
            int totalChannels = _image.Width * _image.Height * 3;
            var usedChannels = new HashSet<int>();
            var allBits = new List<int>();

            // Extract (prefix + message) bits
            while (true)
            {
                int channelIndex;
                do
                {
                    channelIndex = _prng.Next(totalChannels);
                } while (!usedChannels.Add(channelIndex));

                int pixelIndex = channelIndex / 3;
                int channelOffset = channelIndex % 3;

                int x = pixelIndex % _image.Width;
                int y = pixelIndex / _image.Width;

                var pixel = _image[x, y];
                byte value = channelOffset switch
                {
                    0 => pixel.R,
                    1 => pixel.G,
                    2 => pixel.B,
                    _ => throw new InvalidOperationException("Unexpected decoding error — possibly corrupt or not a PNG file.")
                };

                int bit = value & 1;
                allBits.Add(bit);

                // Once we have at least 32 bits, extract prefix length
                if (allBits.Count == StegoConstants.PrefixLength * 8)
                {
                    var prefixBytes = BitsToBytes(allBits.Take(32));
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(prefixBytes);
                    int messageLength = BitConverter.ToInt32(prefixBytes, 0);
                    int totalBits = StegoConstants.PrefixLength * 8 + messageLength * 8;
                    if (totalBits > totalChannels)
                        throw new InvalidOperationException("Decoded length exceeds image capacity — possibly wrong password.");
                    while (allBits.Count < totalBits)
                    {
                        do
                        {
                            channelIndex = _prng.Next(totalChannels);
                        } while (!usedChannels.Add(channelIndex));

                        pixelIndex = channelIndex / 3;
                        channelOffset = channelIndex % 3;

                        x = pixelIndex % _image.Width;
                        y = pixelIndex / _image.Width;

                        pixel = _image[x, y];
                        value = channelOffset switch
                        {
                            0 => pixel.R,
                            1 => pixel.G,
                            2 => pixel.B,
                            _ => throw new InvalidOperationException("Unexpected decoding error — possibly corrupt or not a PNG file.")
                        };

                        bit = value & 1;
                        allBits.Add(bit);
                    }

                    var messageBits = allBits.Skip(32);
                    return BitsToBytes(messageBits);
                }
            }
        }

        private byte[] BitsToBytes(IEnumerable<int> bits)
        {
            var bitList = bits.ToList();
            byte[] bytes = new byte[bitList.Count / 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int bitIndex = 7; bitIndex >= 0; bitIndex--)
                {
                    int bitPos = (i * 8) + (7 - bitIndex);
                    bytes[i] |= (byte)(bitList[bitPos] << bitIndex);
                }
            }
            return bytes;
        }
    }
}