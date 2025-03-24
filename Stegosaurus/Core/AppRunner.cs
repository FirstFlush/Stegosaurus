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
            string? resolvedFilePath = null;
            try
            {
                resolvedFilePath = CliInputHandler.ResolveFilePath(filePath);
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
            return resolvedFilePath;
        }

        public string ResolveOutfilePath(string outfilePath, bool requirePng)
        {
            string? resolvedOutFilePath = null;
            try
            {
                resolvedOutFilePath = CliInputHandler.ResolveOutfilePath(outfilePath, requirePng);
            }
            catch (ArgumentException ex)
            {
                if (requirePng)
                {
                    _logger.LogError("Outfile must be a .png file");
                }
                else
                {
                    _logger.LogError("An unknown error occured while resolving outfile path: {Message}", ex.Message);
                }
                Environment.Exit(1);
            }
            return resolvedOutFilePath;
        }

        private string GenerateOutfilePath(string filePath, bool isEncrypt)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = isEncrypt ? Path.GetExtension(filePath) : ".txt";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"); 
            string newFileName = $"{fileName}__{timestamp}{extension}";
            return Path.Combine(Environment.CurrentDirectory, newFileName);
        }

        private void SaveEncodedFile(Image<Rgba32> image, string outfilePath)
        {
            try
            {
                image.Save(outfilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error while saving file: {Message}", ex.Message);
                Environment.Exit(1);
            }
        }

        private void SaveDecodedFile(string outfilePath, string message)
        {
            try
            {
                File.WriteAllText(outfilePath, message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error while saving file to {Outfile}: {Message}", outfilePath, ex.Message);
                Environment.Exit(1);
            }
        }

        private string ResolvePassword(string password)
        {
            while (string.IsNullOrWhiteSpace(password))
                password = CliInputHandler.ReadPassword();
            return password;
        }

        public void RunEncrypt(string filePath, string password, string message, string? outfilePath)
        {
            filePath = ResolveFilePath(filePath);
            password = ResolvePassword(password);
            if (string.IsNullOrWhiteSpace(outfilePath))
            {
                outfilePath = GenerateOutfilePath(filePath, true);
            }
            outfilePath = ResolveOutfilePath(outfilePath, true);
            
            try 
            {
                var encryptedMsg = AesCryptoService.Encrypt(password, message);
                var encoder = new LsbEncoder(filePath, encryptedMsg, password);
                Image<Rgba32> encodedImage = encoder.Encode();
                SaveEncodedFile(encodedImage, outfilePath);
                _logger.LogInformation("Message successfully encrypted and encoded.");
                _logger.LogInformation("Saved outfile to: {Outfile}", outfilePath);
            }
            catch (InvalidOperationException)
            {
                _logger.LogError("Image file is too small to encode this message.");
                Environment.Exit(1);
            }
            catch (TimeoutException)
            {
                _logger.LogError("Encoding is taking too long. This means the image file is too small, and the PRNG (pseudo-random number generator) is taking too long to find available bits. Try using a larger image.");
                Environment.Exit(1);
            }
            catch (UnknownImageFormatException)
            {
                _logger.LogError("Can not load image file. Ensure you are using a PNG file. If you are using a PNG then your image may be corrupted.");
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error has occurred: {Message}", ex.Message);
                Environment.Exit(1);
            }

        }

        public void RunDecrypt(string filePath, string password, string? outfilePath)
        {
            filePath = ResolveFilePath(filePath);
            if (string.IsNullOrWhiteSpace(outfilePath))
            {
                outfilePath = GenerateOutfilePath(filePath, false);
            }
            outfilePath = ResolveOutfilePath(outfilePath, false);
            password = ResolvePassword(password);

            try
            {
                var decoder = new LsbDecoder(filePath, password);
                byte[] decodedMessage = decoder.Decode();
                string message = AesCryptoService.Decrypt(password, decodedMessage);
                SaveDecodedFile(outfilePath, message);
                _logger.LogInformation("Message successfully saved to: {Outfile}", outfilePath);
            }
            catch (UnknownImageFormatException)
            {
                _logger.LogError("Can not load image file. Ensure you are using a PNG file. If you are using a PNG then your image may be corrupted.");
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                _logger.LogError("Decoding failed due to the following exception: {Message}", ex.Message);
                Environment.Exit(1);
            }
        }
    }
}