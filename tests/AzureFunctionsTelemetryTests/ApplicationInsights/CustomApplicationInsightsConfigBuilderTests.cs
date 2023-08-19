namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class CustomApplicationInsightsConfigBuilderTests
{
    private const string DefaultConfigurationSectionName = "ApplicationInsights";

    [Fact]
    public void GivenApplicationNameProvided_ThenSetConfig()
    {
        // Arrange
        var builder = new CustomApplicationInsightsConfigBuilder("some-name", typeof(StringHelper));

        // Act
        var actualConfig = builder.Build();

        // Assert
        var expectedConfig =
            new CustomApplicationInsightsConfig("some-name", typeof(StringHelper), DefaultConfigurationSectionName);
        actualConfig.Should().BeEquivalentTo(expectedConfig);
    }

    [Fact]
    public void GivenApplicationNameNotProvided_ThenDontSetApplicationName()
    {
        // Arrange
        var builder = new CustomApplicationInsightsConfigBuilder(typeof(StringHelper));

        // Act
        var actualConfig = builder.Build();

        // Assert
        var expectedConfig =
            new CustomApplicationInsightsConfig(applicationName: null, typeof(StringHelper),
                DefaultConfigurationSectionName);
        actualConfig.Should().BeEquivalentTo(expectedConfig);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void GivenEmptyOrWhiteSpaceSectionConfigurationName_ThenThrows(string configurationSectionName)
    {
        // Arrange
        var builder = new CustomApplicationInsightsConfigBuilder("some-name", typeof(StringHelper));

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => builder.WithConfigurationSectionName(configurationSectionName));
    }
}
