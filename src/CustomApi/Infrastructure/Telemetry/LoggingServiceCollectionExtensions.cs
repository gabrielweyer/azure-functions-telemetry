using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CustomApi.Infrastructure.Telemetry
{
    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddConsoleLogging(this IServiceCollection services)
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

            static void DiscardFunctionsConsoleLoggingProvider(LoggerFilterOptions o)
            {
                o.Rules.Add(new LoggerFilterRule(
                    "Azure.Functions.Cli.Diagnostics.ColoredConsoleLoggerProvider",
                    "*",
                    LogLevel.None,
                    null));
            }

            static void DiscardDuplicateExceptionForConsoleLoggingProvider(LoggerFilterOptions o)
            {
                o.Rules.Add(new LoggerFilterRule(
                    "Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider",
                    "Host.Results",
                    LogLevel.None,
                    null));
            }
        }
    }
}