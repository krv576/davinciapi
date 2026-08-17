using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DavinciEPA.Infrastructure.Logging;

/// <summary>Configures Serilog as the logging provider. Operates on <see cref="IHostBuilder"/> so this project never needs an ASP.NET Core framework reference.</summary>
public static class SerilogLoggingExtensions
{
    public static IHostBuilder UseDavinciSerilogLogging(this IHostBuilder hostBuilder, string applicationName) =>
        hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", applicationName)
                .WriteTo.Console();
        });
}
