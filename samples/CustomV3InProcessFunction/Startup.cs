using Custom.FunctionsTelemetry.ApplicationInsights;
using Custom.FunctionsTelemetry.Logging;
using CustomV3InProcessFunction;
using CustomV3InProcessFunction.Functions.UserSecret;
using CustomV3InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace CustomV3InProcessFunction
{
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
                    "customv3inprocess",
                    typeof(Startup))
                .WithHealthRequestFilter("HealthFunction")
                .WithServiceBusTriggerFilter()
                .Build();

            builder.Services
                .AddApplicationInsightsTelemetryProcessor<CustomHttpDependencyFilter>()
                .AddApplicationInsightsTelemetryProcessor<TelemetryCounterProcessor>()
                .AddSingleton<ITelemetryInitializer, TelemetryCounterInitializer>()
                /*
                 * When adding a instance of a telemetry initializer, you need to provide the service Type otherwise
                 * your initializer will not be used.
                 *
                 * <code>
                 * // Dot not use:
                 * .AddSingleton(new TelemetryCounterInstanceInitializer("NiceValue"))
                 * </code>
                 */
                .AddSingleton<ITelemetryInitializer>(new TelemetryCounterInstanceInitializer("NiceValue"))
                .AddCustomApplicationInsights(appInsightsOptions)
                .AddCustomConsoleLogging();
        }
    }
}