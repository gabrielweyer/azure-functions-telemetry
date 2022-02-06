using DefaultV3InProcessFunction;
using DefaultV3InProcessFunction.Functions.UserSecret;
using DefaultV3InProcessFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace DefaultV3InProcessFunction
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<SecretOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Secret").Bind(settings);
                });

            var applicationDescriptor = new ApplicationDescriptor
            {
                Name = "defaultv3inprocess",
                Version = "local"
            };

            builder.Services.AddSingleton(applicationDescriptor);
            builder.Services.AddSingleton<ITelemetryInitializer, ApplicationInitializer>();

            builder.Services.AddApplicationInsightsTelemetryProcessor<SomeSortOfFilter>();
        }
    }
}