using System.CommandLine;

namespace Stegosaurus.CLI
{
    public static class ArgParser
    {
        public static RootCommand BuildCommand()
        {
            var fileOption = new Option<string>(["-f", "--file"], "Path to the PNG image") { IsRequired = true };
            var encryptOption = new Option<bool>(["-e", "--encrypt"], "Encrypt the file") { IsRequired = false };
            var decryptOption = new Option<bool>(["-d", "--decrypt"], "Decrypt the file") { IsRequired = false };
            var passwordOption = new Option<string>(["-p", "--password"], "Password") { IsRequired = false };

            var rootCommand = new RootCommand("Stegosarus CLI");
            var options = new Option[] { fileOption, encryptOption, decryptOption, passwordOption };
            foreach (var opt in options) 
                rootCommand.AddOption(opt);

            return rootCommand;
            // rootCommand.SetHandler((string filePath) =>
            // {
            //     Console.WriteLine($"File provided: {filePath}");
            // }, fileOption);

        }
    }
}