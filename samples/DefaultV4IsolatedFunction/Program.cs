using Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Functions.UserSecret;
using Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Infrastructure;
using Gabo.AzureFunctionTelemetry.Samples.DefaultV4IsolatedFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddUserSecrets<Program>();
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services
            .AddMyOptions<SecretOptions>("Secret")
            .AddApplicationInsightsTelemetryProcessor<TelemetryCounterProcessor>()
            .AddApplicationInsightsTelemetryProcessor<HealthRequestFilter>()
            .AddSingleton<ITelemetryInitializer, TelemetryCounterInitializer>();

        services.Configure<LoggerFilterOptions>(options =>
        {
            // See https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#start-up-and-configuration
            var toRemove = options.Rules.FirstOrDefault(rule => string.Equals(rule.ProviderName
                , "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider",
                StringComparison.Ordinal));
            if (toRemove is not null)
            {
                options.Rules.Remove(toRemove);
            }
        });
    })
    .Build();

await host.RunAsync();
