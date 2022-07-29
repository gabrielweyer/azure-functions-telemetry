using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gabo.AzureFunctionsTelemetry.Logging;

/// <summary>
/// This won't actually be displayed
/// </summary>
public static class LoggingServiceCollectionExtensions
{
    /// <summary>
    /// Replaces the Azure Functions Core Tools Console Logger by the .NET Core Console Logger. This gives us exception
    /// stacktrace in the console. Discards as many duplicate exceptions as can be safely done.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> holding all your precious types.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance but with the .NET Core Console Logger being
    /// registered and a few additional <see cref="LoggerFilterRule"/> to discard the built-in console
    /// logging.</returns>
    public static IServiceCollection AddCustomConsoleLogging(this IServiceCollection services)
    {
        services.AddLogging(b =>
        {
            b.AddConsole();
            services.Configure<LoggerFilterOptions>(o =>
            {
                DiscardFunctionsConsoleLoggingProvider(o);
                DiscardDuplicateExceptionForConsoleLoggingProvider(o);
            });
        });

        return services;

        void DiscardFunctionsConsoleLoggingProvider(LoggerFilterOptions o)
        {
            o.Rules.Add(new LoggerFilterRule(
                "Azure.Functions.Cli.Diagnostics.ColoredConsoleLoggerProvider",
                "*",
                LogLevel.None,
                null));
        }

        void DiscardDuplicateExceptionForConsoleLoggingProvider(LoggerFilterOptions o)
        {
            o.Rules.Add(new LoggerFilterRule(
                "Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider",
                FunctionRuntimeCategory.HostResults,
                LogLevel.None,
                null));
        }
    }
}
