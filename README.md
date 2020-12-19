# Azure Functions Limitations

## Prerequisites

- [Azurite][azurite] is used as the Azure blob emulator
- [Azure Functions Core Tools][azure-functions-core-tools] if you want to run from the command line

## Versions

I used the Azure Functions Core Tools version `3.0.3160` to create the Function App (released on the 9th of December 2020).

NuGet packages:

- `Microsoft.NET.Sdk.Functions`: `3.0.7`

## Default Function App

`DefaultApi` demonstrate the limitations of Azure Functions.

I've decided to commit the `local.settings.json` file. This is not the default or recommended approach but it makes it easier for new joiners to get started.

[azurite]: https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite
[azure-functions-core-tools]: https://github.com/Azure/azure-functions-core-tools
