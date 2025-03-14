using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class ApplicationInsightsServiceCollectionExtensionsTests
{
    [Fact]
    public void GivenApplicationNameNotProvided_ThenDontRegisterApplicationInitializer()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var config = new CustomApplicationInsightsConfig(applicationName: null, typeof(StringHelper), "Q");

        // Act
        var updatedServiceCollection = serviceCollection.AddCustomApplicationInsights(config);

        // Assert
        var registeredTelemetryInitializers = GetTelemetryInitializers(updatedServiceCollection);
        registeredTelemetryInitializers.Should().NotContain(typeof(ApplicationInitializer));
    }

    [Fact]
    public void GivenApplicationNameProvided_ThenRegisterApplicationInitializer()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var config = new CustomApplicationInsightsConfig("app-name", typeof(StringHelper), "Q");

        // Act
        var updatedServiceCollection = serviceCollection.AddCustomApplicationInsights(config);

        // Assert
        var registeredTelemetryInitializers = GetTelemetryInitializers(updatedServiceCollection);
        registeredTelemetryInitializers.Should().Contain(typeof(ApplicationInitializer));
    }

    private static List<Type?> GetTelemetryInitializers(IServiceCollection serviceCollection)
    {
        return serviceCollection
            .Where(s => s.ServiceType == typeof(ITelemetryInitializer) && s.ImplementationType != null)
            .Select(s => s.ImplementationType)
            .ToList();
    }
}
