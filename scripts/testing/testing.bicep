@description('Location for all resources.')
param location string = resourceGroup().location

@description('Used to create a unique name. For example, with a "hello" prefix and an Application Insights resource, the resource name will be "hello-appi".')
@maxLength(48)
param resourceNamePrefix string

var applicationInsightsSettings = {
  name: '${resourceNamePrefix}-appi'
  workspaceName: '${resourceNamePrefix}-log'
}

var defaultV3InProcessFunctionDisplayName = 'defaultV3InProcess'
var defaultV4InProcessFunctionDisplayName = 'defaultV4InProcess'
var customV3InProcessFunctionDisplayName = 'customV3InProcess'
var customV4InProcessFunctionDisplayName = 'customV4InProcess'

var serviceBusSettings = {
  name: '${resourceNamePrefix}sb'
  queueNames: [
    '${toLower(defaultV3InProcessFunctionDisplayName)}-queue'
    '${toLower(defaultV3InProcessFunctionDisplayName)}-exception-queue'
    '${toLower(defaultV4InProcessFunctionDisplayName)}-queue'
    '${toLower(defaultV4InProcessFunctionDisplayName)}-exception-queue'
    '${toLower(customV3InProcessFunctionDisplayName)}-queue'
    '${toLower(customV3InProcessFunctionDisplayName)}-exception-queue'
    '${toLower(customV4InProcessFunctionDisplayName)}-queue'
    '${toLower(customV4InProcessFunctionDisplayName)}-exception-queue'
  ]
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
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

resource queues 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = [for queue in serviceBusSettings.queueNames: {
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
