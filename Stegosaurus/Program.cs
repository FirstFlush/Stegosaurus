
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using Stegosaurus.CLI;
using Stegosaurus.Config;
using Stegosaurus.Core;

// using var serviceProvider = LoggingConfig.ConfigureLogging();

// var rootCommand = ArgParser.BuildCommand();

var serviceProvider = LoggingConfig.ConfigureLogging();
var logger = serviceProvider.GetRequiredService<ILogger<AppRunner>>();

var appRunner = new AppRunner(logger);
var rootCommand = CliOptionsBuilder.BuildCommand(appRunner);

await rootCommand.InvokeAsync(args);
