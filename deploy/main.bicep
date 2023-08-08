@description('Location for all resources.')
param location string = resourceGroup().location

@description('Used to create a unique name. For example, with a "hello" prefix and an Application Insights resource, the resource name will be "hello-appi".')
@maxLength(5)
param resourceNamePrefix string

@description('Used to populate "Secret:ReallySecretValue".')
@secure()
param reallySecretValue string

@description('Expected to be unique, used to name nested deployments.')
param deploymentNameSuffix string

var applicationInsightsSettings = {
  name: '${resourceNamePrefix}-appi'
  workspaceName: '${resourceNamePrefix}-log'
}

var defaultV3InProcessFunctionDisplayName = 'defaultV3InProcess'
var defaultV4InProcessFunctionDisplayName = 'defaultV4InProcess'
var defaultV4IsolatedFunctionDisplayName = 'defaultV4Isolated'
var customV3InProcessFunctionDisplayName = 'customV3InProcess'
var customV4InProcessFunctionDisplayName = 'customV4InProcess'

var functions = [
  {
    displayName: defaultV3InProcessFunctionDisplayName
    hostingPlanName: '${resourceNamePrefix}-${toLower(defaultV3InProcessFunctionDisplayName)}-plan'
    functionAppName: '${resourceNamePrefix}-${toLower(defaultV3InProcessFunctionDisplayName)}-func'
    functionRuntimeVersion: 3
    functionWorkerRuntime: 'dotnet'
    storageName: '${resourceNamePrefix}7${toLower(defaultV3InProcessFunctionDisplayName)}'
  }
  {
    displayName: defaultV4InProcessFunctionDisplayName
    hostingPlanName: '${resourceNamePrefix}-${toLower(defaultV4InProcessFunctionDisplayName)}-plan'
    functionAppName: '${resourceNamePrefix}-${toLower(defaultV4InProcessFunctionDisplayName)}-func'
    functionRuntimeVersion: 4
    functionWorkerRuntime: 'dotnet'
    storageName: '${resourceNamePrefix}7${toLower(defaultV4InProcessFunctionDisplayName)}'
  }
  {
    displayName: defaultV4IsolatedFunctionDisplayName
    hostingPlanName: '${resourceNamePrefix}-${toLower(defaultV4IsolatedFunctionDisplayName)}-plan'
    functionAppName: '${resourceNamePrefix}-${toLower(defaultV4IsolatedFunctionDisplayName)}-func'
    functionRuntimeVersion: 4
    functionWorkerRuntime: 'dotnet-isolated'
    storageName: '${resourceNamePrefix}7${toLower(defaultV4IsolatedFunctionDisplayName)}'
  }
  {
    displayName: customV3InProcessFunctionDisplayName
    hostingPlanName: '${resourceNamePrefix}-${toLower(customV3InProcessFunctionDisplayName)}-plan'
    functionAppName: '${resourceNamePrefix}-${toLower(customV3InProcessFunctionDisplayName)}-func'
    functionRuntimeVersion: 3
    functionWorkerRuntime: 'dotnet'
    storageName: '${resourceNamePrefix}7${toLower(customV3InProcessFunctionDisplayName)}'
  }
  {
    displayName: customV4InProcessFunctionDisplayName
    hostingPlanName: '${resourceNamePrefix}-${toLower(customV4InProcessFunctionDisplayName)}-plan'
    functionAppName: '${resourceNamePrefix}-${toLower(customV4InProcessFunctionDisplayName)}-func'
    functionRuntimeVersion: 4
    functionWorkerRuntime: 'dotnet'
    storageName: '${resourceNamePrefix}7${toLower(customV4InProcessFunctionDisplayName)}'
  }
]

var serviceBusSettings = {
  name: '${resourceNamePrefix}sb'
  queueNames: [
    '${toLower(defaultV3InProcessFunctionDisplayName)}-queue'
    '${toLower(defaultV3InProcessFunctionDisplayName)}-exception-queue'
    '${toLower(defaultV4InProcessFunctionDisplayName)}-queue'
    '${toLower(defaultV4InProcessFunctionDisplayName)}-exception-queue'
    '${toLower(defaultV4IsolatedFunctionDisplayName)}-queue'
    '${toLower(defaultV4IsolatedFunctionDisplayName)}-exception-queue'
    '${toLower(customV3InProcessFunctionDisplayName)}-queue'
    '${toLower(customV3InProcessFunctionDisplayName)}-exception-queue'
    '${toLower(customV4InProcessFunctionDisplayName)}-queue'
    '${toLower(customV4InProcessFunctionDisplayName)}-exception-queue'
  ]
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: applicationInsightsSettings.workspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsSettings.name
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: workspace.id
  }
}

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2021-11-01' = {
  name: serviceBusSettings.name
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {}
}

resource queues 'Microsoft.ServiceBus/namespaces/queues@2021-11-01' = [for queue in serviceBusSettings.queueNames: {
  parent: serviceBusNamespace
  name: queue
  properties: {
    lockDuration: 'PT30S'
    maxSizeInMegabytes: 1024
    requiresDuplicateDetection: false
    requiresSession: false
    defaultMessageTimeToLive: 'P1D'
    deadLetteringOnMessageExpiration: false
    enableBatchedOperations: true
    duplicateDetectionHistoryTimeWindow: 'PT10M'
    maxDeliveryCount: 2
    status: 'Active'
    enablePartitioning: false
    enableExpress: false
  }
}]

module functionAppModule './functionApp.bicep' = [for function in functions:  {
  name: '${function.displayName}-${deploymentNameSuffix}'
  params: {
    location: location
    storageName: function.storageName
    hostingPlanName: function.hostingPlanName
    functionAppName: function.functionAppName
    functionRuntimeVersion: function.functionRuntimeVersion
    functionWorkerRuntime: function.functionWorkerRuntime
    applicationInsightsConnectionString: applicationInsights.properties.ConnectionString
    serviceBusConnectionString: listKeys('${serviceBusNamespace.id}/authorizationRules/RootManageSharedAccessKey', serviceBusNamespace.apiVersion).primaryConnectionString
    reallySecretValue: reallySecretValue
  }
}]

output defaultV3InProcessFunctionAppName string = functions[0].functionAppName
output defaultV4InProcessFunctionAppName string = functions[1].functionAppName
output defaultV4IsolatedFunctionAppName string = functions[2].functionAppName
output customV3InProcessFunctionAppName string = functions[3].functionAppName
output customV4InProcessFunctionAppName string = functions[4].functionAppName
output serviceBusNamespace string = serviceBusSettings.name
output applicationInsightsName string = applicationInsightsSettings.name
