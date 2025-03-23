using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Stegosaurus.CLI;
using Stegosaurus.Crypto;
using Stegosaurus.Stego;

namespace Stegosaurus.Core
{
    public class AppRunner
    {

        private readonly ILogger<AppRunner> _logger;

        public AppRunner(ILogger<AppRunner> logger)
        {
            _logger = logger;
        }

        public string ResolveFilePath(string filePath)
        {
            try
            {
                return CliInputHandler.ResolveFilePath(filePath);
            }
            catch (FileNotFoundException)
            {
                _logger.LogError("Invalid file path: {Path}", filePath);
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error while resolving file path: {Message}", ex.Message);
                Environment.Exit(1);
            }
            return null!;
        }

        private void SaveEncodedFile(Image<Rgba32> image, string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss"); 
            string newFileName = $"{fileName}__{timestamp}{extension}";
            string newFilePath = Path.Combine(Environment.CurrentDirectory, newFileName);
            try
            {
                image.Save(newFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error while saving file: {Message}", ex.Message);
                Environment.Exit(1);
            }
        }

        private string ResolvePassword(string password)
        {
            while (string.IsNullOrWhiteSpace(password))
                password = CliInputHandler.ReadPassword();
            return password;
        }

        public void RunEncrypt(string filePath, string password, string message)
        {
            filePath = ResolveFilePath(filePath);
            password = ResolvePassword(password);
            Image<Rgba32>? encodedImage = null;

            var encryptedMsg = AesCryptoService.Encrypt(password, message);
            var encoder = new LsbEncoder(filePath, encryptedMsg, password);
            
            try 
            {
                encodedImage = encoder.Encode();
            }
            catch (InvalidOperationException)
            {
                _logger.LogError("Image file is too small to encode this message.");
                Environment.Exit(1);
            }
            catch (TimeoutException)
            {
                _logger.LogError("Encoding is taking too long. Try using a larger image.");
                Environment.Exit(1);
            }
            SaveEncodedFile(encodedImage, filePath);
        }

        public void RunDecrypt(string filePath, string password)
        {
            filePath = ResolveFilePath(filePath);
            password = ResolvePassword(password);
        }

    }
}