@description('Location for all resources.')
param location string = resourceGroup().location

@description('Used to create a unique name. For example, with a "hello" prefix and an Application Insights resource, the resource name will be "hello-appi".')
@maxLength(5)
param resourceNamePrefix string

@description('Used to populate "Secret:ReallySecretValue".')
@secure()
param reallySecretValue string

var applicationInsightsSettings = {
  name: '${resourceNamePrefix}-appi'
  workspaceName: '${resourceNamePrefix}-log'
}

var functions = [
  {
    displayName: 'defaultV3InProcess'
    hostingPlanName: '${resourceNamePrefix}-defaultv3inprocess-plan'
    functionAppName: '${resourceNamePrefix}-defaultv3inprocess-func'
    storageName: '${resourceNamePrefix}7defaultv3inprocess'
  }
  {
    displayName: 'defaultV4InProcess'
    hostingPlanName: '${resourceNamePrefix}-defaultv4inprocess-plan'
    functionAppName: '${resourceNamePrefix}-defaultv4inprocess-func'
    storageName: '${resourceNamePrefix}7defaultv4inprocess'
  }
  {
    displayName: 'customV3InProcess'
    hostingPlanName: '${resourceNamePrefix}-customv3inprocess-plan'
    functionAppName: '${resourceNamePrefix}-customv3inprocess-func'
    storageName: '${resourceNamePrefix}7customv3inprocess'
  }
  {
    displayName: 'customV4InProcess'
    hostingPlanName: '${resourceNamePrefix}-customv4inprocess-plan'
    functionAppName: '${resourceNamePrefix}-customv4inprocess-func'
    storageName: '${resourceNamePrefix}7customv4inprocess'
  }
]

var serviceBusSettings = {
  name: '${resourceNamePrefix}sb'
  queueNames: [
    'defaultv3inprocess-queue'
    'defaultv3inprocess-exception-queue'
    'defaultv4inprocess-queue'
    'defaultv4inprocess-exception-queue'
    'customv3inprocess-queue'
    'customv3inprocess-exception-queue'
    'customv4inprocess-queue'
    'customv4inprocess-exception-queue'
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

module functionAppModule './functionApp.bicep' = [for function in functions:  {
  name: '${function.displayName}FunctionAppDeploy'
  params: {
    location: location
    storageName: function.storageName
    hostingPlanName: function.hostingPlanName
    functionAppName: function.functionAppName
    applicationInsightsConnectionString: applicationInsights.properties.ConnectionString
    serviceBusConnectionString: listKeys('${serviceBusNamespace.id}/authorizationRules/RootManageSharedAccessKey', '2021-06-01-preview').primaryConnectionString
    reallySecretValue: reallySecretValue
  }
}]

output defaultV3InProcessFunctionAppName string = functions[0].functionAppName
output defaultV4InProcessFunctionAppName string = functions[1].functionAppName
output customV3InProcessFunctionAppName string = functions[2].functionAppName
output customV4InProcessFunctionAppName string = functions[3].functionAppName
output serviceBusNamespace string = serviceBusSettings.name
output applicationInsightsName string = applicationInsightsSettings.name
