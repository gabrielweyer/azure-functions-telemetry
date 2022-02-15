using System.Collections.Generic;
using Custom.FunctionsTelemetry.ApplicationInsights;
using Custom.FunctionsTelemetry.Logging;
using CustomV4InProcessFunction;
using CustomV4InProcessFunction.Functions.UserSecret;
using CustomV4InProcessFunction.Infrastructure.Telemetry;
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
            .AddApplicationInsightsTelemetryProcessor<TelemetryCounter>()
            .AddCustomApplicationInsights(appInsightsOptions)
            .AddCustomConsoleLogging();
    }
}