using Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction;
using Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Functions.UserSecret;
using Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Infrastructure;
using Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Gabo.AzureFunctionTelemetry.Samples.DefaultV4InProcessFunction;

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

        builder.Services
            .AddApplicationInsightsTelemetryProcessor<TelemetryCounterProcessor>()
            .AddSingleton<ITelemetryInitializer, TelemetryCounterInitializer>();
    }
}
