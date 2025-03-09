using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

using var serviceProvider = new ServiceCollection()
    .AddLogging(builder => 
    {
        builder.ClearProviders();
        builder.AddSerilog();
    }).BuildServiceProvider();