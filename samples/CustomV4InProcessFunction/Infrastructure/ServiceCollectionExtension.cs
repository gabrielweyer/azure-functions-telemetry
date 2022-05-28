using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gabo.AzureFunctionTelemetry.Samples.CustomV4InProcessFunction.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddMyOptions<TOptions>(this IServiceCollection services, string sectionName)
        where TOptions : class
    {
        services.AddOptions<TOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(sectionName).Bind(settings);
            });

        return services;
    }
}
