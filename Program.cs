using System.CommandLine;
using Stegosaurus.CLI;
using Stegosaurus.Config;


using var serviceProvider = LoggingConfig.ConfigureLogging();

var rootCommand = ArgParser.BuildCommand();
await rootCommand.InvokeAsync(args);