using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;


namespace Stegosaurus.Config 
{
    public static class LoggingConfig
    {
        public static ServiceProvider ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            return new ServiceCollection()
                .AddLogging(builder => 
                {
                    builder.ClearProviders();
                    builder.AddSerilog();
                })
                .BuildServiceProvider();
        }
    }
}