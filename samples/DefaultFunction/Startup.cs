using DefaultFunction;
using DefaultFunction.Functions.UserSecret;
using DefaultFunction.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace DefaultFunction
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
                Name = "default-worker",
                Version = "local"
            };

            builder.Services.AddSingleton(applicationDescriptor);
            builder.Services.AddSingleton<ITelemetryInitializer, ApplicationInitializer>();

            builder.Services.AddApplicationInsightsTelemetryProcessor<SomeSortOfFilter>();
        }
    }
}