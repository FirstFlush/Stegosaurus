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
            var outfileEncryptOption = new Option<string>(["-o", "--outfile"], "Optional path to save the encoded PNG file") { IsRequired = false};
            var outfileDecryptOption = new Option<string>(["-o", "--outfile"], "Optional path to save the extracted message") { IsRequired = false};

            var globalOptions = new Option[] { fileOption, passwordOption };
            foreach (var opt in globalOptions)
            {
                encryptCommand.AddOption(opt);
                decryptCommand.AddOption(opt);
            }
            encryptCommand.AddOption(messageOption);
            encryptCommand.AddOption(outfileEncryptOption);
            decryptCommand.AddOption(outfileDecryptOption);
            encryptCommand.SetHandler(appRunner.RunEncrypt, fileOption, passwordOption, messageOption, outfileEncryptOption);
            decryptCommand.SetHandler(appRunner.RunDecrypt, fileOption, passwordOption, outfileDecryptOption);

            rootCommand.AddCommand(encryptCommand);
            rootCommand.AddCommand(decryptCommand);

            return rootCommand;
        }
    }
}