namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal static class ListOfTelemetryItemExtensions
{
    public static List<T> GetOperationItems<T>(this List<TelemetryItem> items, string operationName, string operationId) where T : TelemetryItem
    {
        return items
            .Where(i => i is T)
            .Cast<T>()
            .Where(i => operationId.Equals(i.OperationId, StringComparison.Ordinal) &&
                        (operationName.Equals(i.OperationName, StringComparison.Ordinal) || string.IsNullOrEmpty(i.OperationName)))
            .ToList();
    }
}
