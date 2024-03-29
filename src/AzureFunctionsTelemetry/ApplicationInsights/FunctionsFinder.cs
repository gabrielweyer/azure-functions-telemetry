namespace Gabo.AzureFunctionsTelemetry.ApplicationInsights;

internal static class FunctionsFinder
{
    /// <summary>
    /// This is a quick and dirty attempt to identity all Service Bus triggered Functions so that we can discard the
    /// telemetry without requiring the consumer to maintain a list of Functions.
    /// </summary>
    /// <param name="typeFromEntryAssembly"></param>
    /// <returns>In case of failure, we return an empty list as it's better to emit additional telemetry rather
    /// than breaking Application Insights configuration.</returns>
    public static List<string> GetServiceBusTriggeredFunctionNames(Type typeFromEntryAssembly)
    {
        try
        {
            return (from exportedType in typeFromEntryAssembly.Assembly.ExportedTypes
                    from method in exportedType.GetMethods()
                    from parameter in method.GetParameters()
                    let parameterCustomAttributes = parameter.CustomAttributes
                    from parameterCustomAttribute in parameterCustomAttributes
                    where "Microsoft.Azure.WebJobs.ServiceBusTriggerAttribute"
                        .Equals(parameterCustomAttribute.AttributeType.FullName, StringComparison.Ordinal)
                    from methodCustomAttribute in method.CustomAttributes
                    where "Microsoft.Azure.WebJobs.FunctionNameAttribute"
                        .Equals(methodCustomAttribute.AttributeType.FullName, StringComparison.Ordinal)
                    select (string?)methodCustomAttribute.ConstructorArguments.First().Value)
                .WhereNotNull()
                .ToList();
        }
#pragma warning disable CA1031 // Catch-all and swallow. Better to emit additional telemetry rather than breaking Application Insights
        catch
#pragma warning restore CA1031
        {
            return new List<string>();
        }
    }

    private static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        return source.Where(item => item != null)!;
    }
}
