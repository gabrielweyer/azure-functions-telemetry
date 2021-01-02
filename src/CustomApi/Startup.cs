using CustomApi;
using CustomApi.Infrastructure.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace CustomApi
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
                Name = "custom-api",
                Version = "local"
            };

            builder.Services.AddSingleton(applicationDescriptor);
            builder.Services.AddSingleton<ITelemetryInitializer, ApplicationInitializer>();

            builder.Services.AddApplicationInsightsTelemetryProcessor<SomeSortOfFilter>();
        }
    }
}