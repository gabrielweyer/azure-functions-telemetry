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
var defaultV3InProcessFunctionAppSettings = {
  hostingPlanName: '${resourceNamePrefix}-defaultv3inprocess-plan'
  functionAppName: '${resourceNamePrefix}-defaultv3inprocess-func'
  storageName: '${resourceNamePrefix}7defaultv3inprocess'
  queue: 'defaultv3inprocess-queue'
  exceptionQueue: 'defaultv3inprocess-exception-queue'
}
var defaultV4InProcessFunctionAppSettings = {
  hostingPlanName: '${resourceNamePrefix}-defaultv4inprocess-plan'
  functionAppName: '${resourceNamePrefix}-defaultv4inprocess-func'
  storageName: '${resourceNamePrefix}7defaultv4inprocess'
  queue: 'defaultv4inprocess-queue'
  exceptionQueue: 'defaultv4inprocess-exception-queue'
}
var customV3InProcessFunctionAppSettings = {
  hostingPlanName: '${resourceNamePrefix}-customv3inprocess-plan'
  functionAppName: '${resourceNamePrefix}-customv3inprocess-func'
  storageName: '${resourceNamePrefix}7customv3inprocess'
  queue: 'customv3inprocess-queue'
  exceptionQueue: 'customv3inprocess-exception-queue'
}
var customV4InProcessFunctionAppSettings = {
  hostingPlanName: '${resourceNamePrefix}-customv4inprocess-plan'
  functionAppName: '${resourceNamePrefix}-customv4inprocess-func'
  storageName: '${resourceNamePrefix}7customv4inprocess'
  queue: 'customv4inprocess-queue'
  exceptionQueue: 'customv4inprocess-exception-queue'
}
var serviceBusNamespaceSettings = {
  name: '${resourceNamePrefix}sb'
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
  name: serviceBusNamespaceSettings.name
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {}
}

resource defaultV3InProcessQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespaceSettings.name}/${defaultV3InProcessFunctionAppSettings.queue}'
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
  dependsOn: [
    serviceBusNamespace
  ]
}

resource defaultV3InProcessExceptionQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespaceSettings.name}/${defaultV3InProcessFunctionAppSettings.exceptionQueue}'
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
  dependsOn: [
    serviceBusNamespace
  ]
}

resource defaultV4InProcessQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespaceSettings.name}/${defaultV4InProcessFunctionAppSettings.queue}'
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
  dependsOn: [
    serviceBusNamespace
  ]
}

resource defaultV4InProcessExceptionQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespaceSettings.name}/${defaultV4InProcessFunctionAppSettings.exceptionQueue}'
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
  dependsOn: [
    serviceBusNamespace
  ]
}

resource customV3InProcessQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespaceSettings.name}/${customV3InProcessFunctionAppSettings.queue}'
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
  dependsOn: [
    serviceBusNamespace
  ]
}

resource customV3InProcessExceptionQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespaceSettings.name}/${customV3InProcessFunctionAppSettings.exceptionQueue}'
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
  dependsOn: [
    serviceBusNamespace
  ]
}

resource customV4InProcessQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespaceSettings.name}/${customV4InProcessFunctionAppSettings.queue}'
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
  dependsOn: [
    serviceBusNamespace
  ]
}

resource customV4InProcessExceptionQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespaceSettings.name}/${customV4InProcessFunctionAppSettings.exceptionQueue}'
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
  dependsOn: [
    serviceBusNamespace
  ]
}

resource defaultV3InProcessStorageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: defaultV3InProcessFunctionAppSettings.storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
  }
}

resource defaultV3InProcessHostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: defaultV3InProcessFunctionAppSettings.hostingPlanName
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

resource defaultV3InProcessFunctionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: defaultV3InProcessFunctionAppSettings.functionAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: defaultV3InProcessHostingPlan.id
  }
}

resource defaultV3InProcessFunctionAppAppSettings 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${defaultV3InProcessFunctionAppSettings.functionAppName}/appsettings'
  properties: {
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${defaultV3InProcessFunctionAppSettings.storageName};AccountKey=${listKeys(defaultV3InProcessFunctionAppSettings.storageName, '2019-06-01').keys[0].value}'
    APPLICATIONINSIGHTS_CONNECTION_STRING: reference(applicationInsights.id, '2020-02-02').ConnectionString
    FUNCTIONS_EXTENSION_VERSION: '~3'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${defaultV3InProcessFunctionAppSettings.storageName};AccountKey=${listKeys(defaultV3InProcessFunctionAppSettings.storageName, '2019-06-01').keys[0].value}'
    WEBSITE_CONTENTSHARE: defaultV3InProcessFunctionAppSettings.functionAppName
    WEBSITE_RUN_FROM_PACKAGE: '1'
    'Secret:ReallySecretValue': reallySecretValue
    ServiceBusConnection: listKeys(resourceId('Microsoft.ServiceBus/namespaces/authorizationRules', serviceBusNamespaceSettings.name, 'RootManageSharedAccessKey'), '2017-04-01').primaryConnectionString
  }
  dependsOn: [
    defaultV3InProcessStorageAccount
    defaultV3InProcessFunctionApp
    serviceBusNamespace
  ]
}

resource customV3InProcessStorageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: customV3InProcessFunctionAppSettings.storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
  }
}

resource customV3InProcessHostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: customV3InProcessFunctionAppSettings.hostingPlanName
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

resource customV3InProcessFunctionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: customV3InProcessFunctionAppSettings.functionAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: customV3InProcessHostingPlan.id
  }
}

resource customV3InProcessFunctionAppAppSettings 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${customV3InProcessFunctionAppSettings.functionAppName}/appsettings'
  properties: {
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${customV3InProcessFunctionAppSettings.storageName};AccountKey=${listKeys(customV3InProcessFunctionAppSettings.storageName, '2019-06-01').keys[0].value}'
    APPLICATIONINSIGHTS_CONNECTION_STRING: reference(applicationInsights.id, '2020-02-02').ConnectionString
    FUNCTIONS_EXTENSION_VERSION: '~3'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${customV3InProcessFunctionAppSettings.storageName};AccountKey=${listKeys(customV3InProcessFunctionAppSettings.storageName, '2019-06-01').keys[0].value}'
    WEBSITE_CONTENTSHARE: customV3InProcessFunctionAppSettings.functionAppName
    WEBSITE_RUN_FROM_PACKAGE: '1'
    'Secret:ReallySecretValue': reallySecretValue
    ServiceBusConnection: listKeys(resourceId('Microsoft.ServiceBus/namespaces/authorizationRules', serviceBusNamespaceSettings.name, 'RootManageSharedAccessKey'), '2017-04-01').primaryConnectionString
  }
  dependsOn: [
    customV3InProcessStorageAccount
    customV3InProcessFunctionApp
    serviceBusNamespace
  ]
}

resource defaultV4InProcessStorageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: defaultV4InProcessFunctionAppSettings.storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
  }
}

resource defaultV4InProcessHostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: defaultV4InProcessFunctionAppSettings.hostingPlanName
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

resource defaultV4InProcessFunctionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: defaultV4InProcessFunctionAppSettings.functionAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: defaultV4InProcessHostingPlan.id
  }
}

resource defaultV4InProcessFunctionAppAppSettings 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${defaultV4InProcessFunctionAppSettings.functionAppName}/appsettings'
  properties: {
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${defaultV4InProcessFunctionAppSettings.storageName};AccountKey=${listKeys(defaultV4InProcessFunctionAppSettings.storageName, '2021-06-01').keys[0].value}'
    APPLICATIONINSIGHTS_CONNECTION_STRING: reference(applicationInsights.id, '2020-02-02').ConnectionString
    FUNCTIONS_EXTENSION_VERSION: '~4'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${defaultV4InProcessFunctionAppSettings.storageName};AccountKey=${listKeys(defaultV4InProcessFunctionAppSettings.storageName, '2021-06-01').keys[0].value}'
    WEBSITE_CONTENTSHARE: defaultV4InProcessFunctionAppSettings.functionAppName
    WEBSITE_RUN_FROM_PACKAGE: '1'
    'Secret:ReallySecretValue': reallySecretValue
    ServiceBusConnection: listKeys(resourceId('Microsoft.ServiceBus/namespaces/authorizationRules', serviceBusNamespaceSettings.name, 'RootManageSharedAccessKey'), '2017-04-01').primaryConnectionString
  }
  dependsOn: [
    defaultV4InProcessStorageAccount
    defaultV4InProcessFunctionApp
    serviceBusNamespace
  ]
}

resource customV4InProcessStorageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: customV4InProcessFunctionAppSettings.storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
  }
}

resource customV4InProcessHostingPlanName 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: customV4InProcessFunctionAppSettings.hostingPlanName
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

resource customV4InProcessFunctionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: customV4InProcessFunctionAppSettings.functionAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: customV4InProcessHostingPlanName.id
  }
}

resource customV4InProcessFunctionAppAppSettings 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${customV4InProcessFunctionAppSettings.functionAppName}/appsettings'
  properties: {
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${customV4InProcessFunctionAppSettings.storageName};AccountKey=${listKeys(customV4InProcessFunctionAppSettings.storageName, '2021-06-01').keys[0].value}'
    APPLICATIONINSIGHTS_CONNECTION_STRING: reference(applicationInsights.id, '2020-02-02').ConnectionString
    FUNCTIONS_EXTENSION_VERSION: '~4'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${customV4InProcessFunctionAppSettings.storageName};AccountKey=${listKeys(customV4InProcessFunctionAppSettings.storageName, '2021-06-01').keys[0].value}'
    WEBSITE_CONTENTSHARE: customV4InProcessFunctionAppSettings.functionAppName
    WEBSITE_RUN_FROM_PACKAGE: '1'
    'Secret:ReallySecretValue': reallySecretValue
    ServiceBusConnection: listKeys(resourceId('Microsoft.ServiceBus/namespaces/authorizationRules', serviceBusNamespaceSettings.name, 'RootManageSharedAccessKey'), '2017-04-01').primaryConnectionString
  }
  dependsOn: [
    customV4InProcessStorageAccount
    customV4InProcessFunctionApp
    serviceBusNamespace
  ]
}

output defaultV3InProcessFunctionAppName string = defaultV3InProcessFunctionAppSettings.functionAppName
output defaultV4InProcessFunctionAppName string = defaultV4InProcessFunctionAppSettings.functionAppName
output customV3InProcessFunctionAppName string = customV3InProcessFunctionAppSettings.functionAppName
output customV4InProcessFunctionAppName string = customV4InProcessFunctionAppSettings.functionAppName
output serviceBusNamespace string = serviceBusNamespaceSettings.name
output applicationInsightsName string = applicationInsightsSettings.name
