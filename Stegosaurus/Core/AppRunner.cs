using Microsoft.Extensions.Logging;
using Stegosaurus.CLI;
using Stegosaurus.Crypto;


namespace Stegosaurus.Core
{
    public class AppRunner
    {

        private readonly ILogger<AppRunner> _logger;

        public AppRunner(ILogger<AppRunner> logger)
        {
            _logger = logger;
        }

        public void CheckFilePath(string filePath)
        {
            if (!CliInputHandler.IsValid(filePath))
            {
                _logger.LogError("Invalid file path: {Path}", filePath);
                Environment.Exit(1);
            }
        }

        public void RunEncrypt(string filePath, string password, string message)
        {
            _logger.Log(LogLevel.Information, "running encrypt");

            CheckFilePath(filePath);
            while (string.IsNullOrWhiteSpace(password))
                password = CliInputHandler.ReadPassword();

            Console.WriteLine(password);

            var encryptedMsg = AesCryptoService.Encrypt(password, message);

            Console.WriteLine(string.Join(",", encryptedMsg));
        }

        public void RunDecrypt(string filePath, string password)
        {
            CheckFilePath(filePath);
        }

    }
}