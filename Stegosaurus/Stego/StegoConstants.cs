using System.Security.Cryptography;
using System.Text;


namespace Stegosaurus.Stego
{
    public static class StegoConstants
    {
        public const int PrefixLength = 4;
    
        private static int DerivePrngSeed(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToInt32(hash, 0); // first 4 bytes of hash gets converted to integer
        }

        public static Random Prng(string password)
        {
            var seed = DerivePrngSeed(password);
            return new Random(seed);
        }
    }
}