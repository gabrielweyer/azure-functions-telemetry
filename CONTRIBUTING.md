# Contributing

## Integration tests

Before being able to run the integration tests, you'll need to deploy an Application Insights instance and a Service Bus namespace. This can be done using [scripts\testing\Deploy-Testing.ps1](scripts\testing\Deploy-Testing.ps1).

Once you have provisioned the infrastructure, write down the Service Bus connection string.

### Running the tests in NUKE

You need to set the Service Bus connection string before running `NUKE`:

```powershell
$Env:IntegrationTestServiceBusConnectionString='{testing-connection-string}'
.\build.ps1
```

### Running the tests manually

The integration tests rely on both Functions `CustomV4InProcessFunction` and `DefaultV4InProcessFunction` running locally.

Replace the user secrets:

```powershell
dotnet user-secrets set Testing:IsEnabled true --id 074ca336-270b-4832-9a1a-60baf152b727
dotnet user-secrets set ServiceBusConnection '{testing-connection-string}' --id 074ca336-270b-4832-9a1a-60baf152b727
```

Start the custom v4 in-process Function:

```powershell
cd samples\CustomV4InProcessFunction
func start
```

Start the default v4 in-process Function:

```powershell
cd samples\DefaultV4InProcessFunction
func start
```

Run the [integration tests](tests\AzureFunctionsTelemetryIntegrationTests).

Once you're done, restore the user secrets:

```powershell
dotnet user-secrets remove Testing:IsEnabled 074ca336-270b-4832-9a1a-60baf152b727
dotnet user-secrets set ServiceBusConnection '{connection-string}' --id 074ca336-270b-4832-9a1a-60baf152b727
```

If you want to test the behaviour when the Application Insights connection string is not set, you'll need to remove the secret:

```powershell
dotnet user-secrets remove APPLICATIONINSIGHTS_CONNECTION_STRING --id 074ca336-270b-4832-9a1a-60baf152b727
```

Run the [App Insights connection string integration tests](tests\AppInsightsConnectionStringIntegrationTests).

Once you're done, restore the user secret:

```powershell
dotnet user-secrets set APPLICATIONINSIGHTS_CONNECTION_STRING '{connection-string}' --id 074ca336-270b-4832-9a1a-60baf152b727
```
