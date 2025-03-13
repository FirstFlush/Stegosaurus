using System.CommandLine;

namespace Stegosaurus.CLI
{
    public static class ArgParser
    {
        public static RootCommand BuildCommand()
        {
            var fileOption = new Option<string>(
                [ "-f", "--file" ],
                "Path to the PNG image"
            )
            {
                IsRequired = true
            };

            var rootCommand = new RootCommand("Stegosarus CLI");
            rootCommand.AddOption(fileOption);

            rootCommand.SetHandler((string filePath) =>
            {
                Console.WriteLine($"File provided: {filePath}");
            }, fileOption);

            return rootCommand;
        }
    }
}