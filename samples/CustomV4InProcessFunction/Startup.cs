using Gabo.AzureFunctionsTelemetry.ApplicationInsights;
using Gabo.AzureFunctionsTelemetry.Logging;
using Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction;
using Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Functions.UserSecret;
using Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddOptions<SecretOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("Secret").Bind(settings);
            });

        var appInsightsOptions = new CustomApplicationInsightsOptionsBuilder(
                "customv4inprocess",
                typeof(Startup))
            .WithHealthRequestFilter("HealthFunction")
            .WithServiceBusTriggerFilter()
            .Build();

        builder.Services
            .AddApplicationInsightsTelemetryProcessor<CustomHttpDependencyFilter>()
            .AddApplicationInsightsTelemetryProcessor<TelemetryCounterProcessor>()
            .AddSingleton<ITelemetryInitializer, TelemetryCounterInitializer>()
            /*
             * When adding an instance of a telemetry initializer, you need to provide the service Type otherwise
             * your initializer will not be used.
             *
             * <code>
             * // Do not use:
             * .AddSingleton(new TelemetryCounterInstanceInitializer("NiceValue"))
             * </code>
             */
            .AddSingleton<ITelemetryInitializer>(new TelemetryCounterInstanceInitializer("NiceValue"))
            .AddCustomApplicationInsights(appInsightsOptions)
            .AddCustomConsoleLogging();
    }
}