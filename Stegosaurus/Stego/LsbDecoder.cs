


namespace Stegosaurus.Stego
{

    public class LsbDecoder
    {
        private readonly string _filePath;
        private readonly string _password;

        public LsbDecoder(string filePath, string password)
        {
            _filePath = filePath;
            _password = password;
        }
    }
}