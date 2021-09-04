using System.Collections.Generic;
using Custom.FunctionsTelemetry.ApplicationInsights;
using Custom.FunctionsTelemetry.Logging;
using CustomFunction;
using CustomFunction.Functions.UserSecret;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace CustomFunction
{
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
                    "custom-worker",
                    typeof(Startup))
                .WithHealthRequestFilter("HealthFunction")
                .WithDependencyFilter("CustomHTTP")
                .WithServiceBusRequestInitializer()
                .DiscardServiceBusDuplicateExceptions(serviceBusTriggeredFunctionNames)
                .WithServiceBusTriggerFilter()
                .Build();

            builder.Services
                .AddCustomApplicationInsights(appInsightsOptions)
                .AddCustomConsoleLogging();
        }
    }
}