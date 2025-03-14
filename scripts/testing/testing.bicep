@description('Location for all resources.')
param location string = resourceGroup().location

@description('Used to create a unique name. For example, with a "hello" prefix and an Application Insights resource, the resource name will be "hello-appi".')
@maxLength(48)
param resourceNamePrefix string

var applicationInsightsSettings = {
  name: '${resourceNamePrefix}-appi'
  workspaceName: '${resourceNamePrefix}-log'
}

var defaultV4InProcessFunctionDisplayName = 'defaultV4InProcess'
var customV4InProcessFunctionDisplayName = 'customV4InProcess'

var serviceBusSettings = {
  name: '${resourceNamePrefix}sb'
  queueNames: [
    '${toLower(defaultV4InProcessFunctionDisplayName)}-queue'
    '${toLower(defaultV4InProcessFunctionDisplayName)}-exception-queue'
    '${toLower(customV4InProcessFunctionDisplayName)}-queue'
    '${toLower(customV4InProcessFunctionDisplayName)}-exception-queue'
  ]
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
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

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: serviceBusSettings.name
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {}
}

resource queues 'Microsoft.ServiceBus/namespaces/queues@2024-01-01' = [for queue in serviceBusSettings.queueNames: {
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
    maxDeliveryCount: 1
    status: 'Active'
    enablePartitioning: false
    enableExpress: false
  }
}]
