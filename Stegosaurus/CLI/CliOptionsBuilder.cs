using System.CommandLine;
using Stegosaurus.Core;

namespace Stegosaurus.CLI
{
    public static class CliOptionsBuilder
    {
        public static RootCommand BuildCommand(AppRunner appRunner)
        {
            var rootCommand = new RootCommand("Stegosarus CLI");

            var encryptCommand = new Command("encrypt", "Encrypt a message within a file");
            var decryptCommand = new Command("decrypt", "Decrypt a message within a file");

            var fileOption = new Option<string>(["-f", "--file"], "Path to the PNG image") { IsRequired = true };
            var passwordOption = new Option<string>(["-p", "--password"], "Password") { IsRequired = false };
            var messageOption = new Option<string>(["-m", "--message"], "The message to be encrypted") { IsRequired = true };

            var globalOptions = new Option[] { fileOption, passwordOption };
            foreach (var opt in globalOptions)
            {
                encryptCommand.AddOption(opt);
                decryptCommand.AddOption(opt);
            }
            encryptCommand.AddOption(messageOption);
            encryptCommand.SetHandler(appRunner.RunEncrypt, fileOption, passwordOption, messageOption);
            decryptCommand.SetHandler(appRunner.RunDecrypt, fileOption, passwordOption);

            rootCommand.AddCommand(encryptCommand);
            rootCommand.AddCommand(decryptCommand);

            return rootCommand;
        }
    }
}

            // rootCommand.SetHandler((string filePath) =>
            // {
            //     Console.WriteLine($"File provided: {filePath}");
            // }, fileOption);