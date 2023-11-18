# Isolated mode

The Application Insights integration has been redesigned. Telemetry processors are now supported (unfortunately they're not called for request telemetry items). There is one notable regression: telemetry initializers are not called anymore on request telemetry items.

:rotating_light: if you want to store the Application Insights connection string in [Secret Manager][secret-manager] when running locally, you'll need to reference `Microsoft.Extensions.Configuration.UserSecrets` and call `AddUserSecrets`. Otherwise the Application Insights integration will be partially broken.

Improvements compared to in-process:

- Telemetry processors are supported for all telemetry item types except request
- Console logger displays the exceptions' stack trace
- The integration registers `TelemetryConfiguration` even when the Application Insights connection string is not present

Regressions compared to in-process:

- **Telemetry initializers are not supported on requests**
- Each Function invocation records an `Invoke` dependency

No improvements compared to in-process (but I wish they were):

- The "_Executing ..._" and "_Executed ..._" traces can't be discarded
- Two dependencies are recorded when using Service Bus output binding (`Message` and `ServiceBusSender.Send`)
- Exceptions are still recorded twice for HTTP binding and three times for Service Bus binding

NuGet packages:

- `Microsoft.Azure.Functions.Worker`: `1.20.0` (added automatically when creating the Function, updated later)
- `Microsoft.Azure.Functions.Worker.Sdk`: `1.16.2` (added automatically when creating the Function, updated later)
- `Microsoft.ApplicationInsights.WorkerService`: `2.21.0` (added manually following [Application Insights][direct-app-insights-integration])
- `Microsoft.Azure.Functions.Worker.ApplicationInsights`: `1.1.0` (added manually following [Application Insights][direct-app-insights-integration])

I removed the Application Insights rule that [discarded any traces below Warning][remove-warning-app-insights-rule].

The Function team put together a [sample][isolated-worker-sample].

[remove-warning-app-insights-rule]: https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#start-up-and-configuration
[direct-app-insights-integration]: https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#application-insights
[isolated-worker-sample]: https://github.com/Azure/azure-functions-dotnet-worker/tree/main/samples/FunctionApp
[secret-manager]: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows
