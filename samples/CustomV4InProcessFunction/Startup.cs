using System.Collections.Generic;
using Custom.FunctionsTelemetry.ApplicationInsights;
using Custom.FunctionsTelemetry.Logging;
using CustomV4InProcessFunction;
using CustomV4InProcessFunction.Functions.UserSecret;
using CustomV4InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace CustomV4InProcessFunction;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var serviceBusTriggeredFunctionNames = new List<string>
        {
            "ServiceBusFunction",
            "ServiceBusExceptionThrowingFunction"
        };

        builder.Services.AddOptions<SecretOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("Secret").Bind(settings);
            });

        var appInsightsOptions = new CustomApplicationInsightsOptionsBuilder(
                "customv4inprocess",
                typeof(Startup))
            .WithHealthRequestFilter("HealthFunction")
            .WithServiceBusRequestInitializer()
            .DiscardServiceBusDuplicateExceptions(serviceBusTriggeredFunctionNames)
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