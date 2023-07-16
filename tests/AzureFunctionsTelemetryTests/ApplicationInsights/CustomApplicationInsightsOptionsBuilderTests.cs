namespace Gabo.AzureFunctionsTelemetryTests.ApplicationInsights;

public class CustomApplicationInsightsOptionsBuilderTests
{
    private readonly string _defaultConfigurationSectionName = "ApplicationInsights";

    [Fact]
    public void GivenBasicConfiguration_ThenSetApplicationNameAndEntryType()
    {
        // Arrange
        var builder = new CustomApplicationInsightsConfigBuilder("some-name", typeof(StringHelper));

        // Act
        var actualOptions = builder.Build();

        // Assert
        var expectedOptions =
            new CustomApplicationInsightsConfig("some-name", typeof(StringHelper), _defaultConfigurationSectionName);
        actualOptions.Should().BeEquivalentTo(expectedOptions);
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
