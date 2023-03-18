@description('Location for all resources.')
param location string = resourceGroup().location

@description('Function App storage account name.')
param storageName string

@description('Function App hosting plan name.')
param hostingPlanName string

@description('Function App name.')
param functionAppName string

@description('Function runtime version (v3 or v4).')
@allowed([
  3
  4
])
param functionRuntimeVersion int

@description('Application Insights connection string.')
@secure()
param applicationInsightsConnectionString string

@description('Service Bus connection string.')
@secure()
param serviceBusConnectionString string

@description('Used to populate "Secret:ReallySecretValue".')
@secure()
param reallySecretValue string

var functionsExtensionsVersion = '~${functionRuntimeVersion}'

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: hostingPlanName
  location: location
  kind: ''
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
    capacity: 0
  }
  properties: {}
}

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
  }
}

var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
resource appSettings 'Microsoft.Web/sites/config@2022-03-01' = {
  parent: functionApp
  name: 'appsettings'
  properties: {
    AzureWebJobsStorage: storageAccountConnectionString
    APPLICATIONINSIGHTS_CONNECTION_STRING: applicationInsightsConnectionString
    FUNCTIONS_EXTENSION_VERSION: functionsExtensionsVersion
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: storageAccountConnectionString
    WEBSITE_CONTENTSHARE: functionAppName
    WEBSITE_RUN_FROM_PACKAGE: '1'
    'Secret:ReallySecretValue': reallySecretValue
    ServiceBusConnection: serviceBusConnectionString
    'Testing:IsEnabled': 'false'
  }
}
