# Contributing

## Integration tests

Before being able to run the integration tests, you'll need to deploy an Application Insights instance and a Service Bus namespace. This can be done using [scripts\testing\deploy-testing.ps1](scripts\testing\deploy-testing.ps1).

Once you have provisioned the infrastructure, write down the Service Bus connection string.

### Running the tests in NUKE

You need to set the Service Bus connection string before running `NUKE`:

```powershell
$Env:ServiceBusConnection='{connection-string}'
.\build.ps1
```

### Running the tests manually

The integration tests rely on both Functions `CustomV4InProcessFunction` and `DefaultV4InProcessFunction` running locally.

Start the custom v4 in-process Function:

```powershell
cd samples\CustomV4InProcessFunction
$Env:Testing:IsEnabled='true'
$Env:ServiceBusConnection='{connection-string}'
func start
```

Start the default v4 in-process Function:

```powershell
cd samples\DefaultV4InProcessFunction
$Env:Testing:IsEnabled='true'
$Env:ServiceBusConnection='{connection-string}'
func start
```

Run the [integration tests](tests\AzureFunctionsTelemetryIntegrationTests).
