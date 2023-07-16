using Gabo.AzureFunctionsTelemetry.ApplicationInsights;
using Gabo.AzureFunctionsTelemetry.Logging;
using Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction;
using Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Functions.UserSecret;
using Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Infrastructure;
using Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Channel;
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
        var testingOptions = builder.GetContext().Configuration.GetSection("Testing").Get<TestingOptions>();

        if (testingOptions.IsEnabled)
        {
            builder.Services.AddSingleton<ITelemetryChannel, TestingChannel>();
        }

        builder.Services
            .AddMyOptions<TestingOptions>("Testing")
            .AddMyOptions<SecretOptions>("Secret");

        var appInsightsOptions = new CustomApplicationInsightsConfigBuilder(
                "customv4inprocess",
                typeof(Startup))
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
