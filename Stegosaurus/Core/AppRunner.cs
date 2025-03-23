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
            SaveEncodedFile(encodedImage, outfilePath);
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

            var decoder = new LsbDecoder(filePath, password);
            byte[]? decodedMessage = null; 
            try
            {
                decodedMessage = decoder.Decode();

            }
            catch (Exception ex)
            {
                _logger.LogError("Decoding failed due to the following exception: {Message}", ex.Message);
                Environment.Exit(1);
            }
            string message = AesCryptoService.Decrypt(password, decodedMessage);
            SaveDecodedFile(outfilePath, message);
        }
    }
}