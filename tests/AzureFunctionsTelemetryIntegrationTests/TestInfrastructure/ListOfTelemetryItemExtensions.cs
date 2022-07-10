namespace Gabo.AzureFunctionsTelemetryIntegrationTests.TestInfrastructure;

internal static class ListOfTelemetryItemExtensions
{
    public static List<T> GetOperationItems<T>(this List<TelemetryItem> items, string operationId) where T : TelemetryItem
    {
        return items
            .Where(i => i is T)
            .Cast<T>()
            .Where(i => i.OperationId == operationId)
            .ToList();
    }
}
