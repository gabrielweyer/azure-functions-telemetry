@description('Location for all resources.')
param location string = resourceGroup().location

@description('Used to create a unique name. For example, with a \'hello\' prefix and an Application Insights resource, the resource name will be \'hello-appi\'.')
param resourceNamePrefix string

@description('Used to populate \'Secret:ReallySecretValue\'.')
@secure()
param reallySecretValue string

var applicationInsights = {
  name: '${resourceNamePrefix}-appi'
  workspaceName: '${resourceNamePrefix}-log'
}
var defaultV3InProcessFunctionApp = {
  hostingPlanName: '${resourceNamePrefix}-defaultv3inprocess-plan'
  functionAppName: '${resourceNamePrefix}-defaultv3inprocess-func'
  storageName: '${resourceNamePrefix}7defaultv3inprocess'
  queue: 'defaultv3inprocess-queue'
  exceptionQueue: 'defaultv3inprocess-exception-queue'
}
var defaultV4InProcessFunctionApp = {
  hostingPlanName: '${resourceNamePrefix}-defaultv4inprocess-plan'
  functionAppName: '${resourceNamePrefix}-defaultv4inprocess-func'
  storageName: '${resourceNamePrefix}7defaultv4inprocess'
  queue: 'defaultv4inprocess-queue'
  exceptionQueue: 'defaultv4inprocess-exception-queue'
}
var customV3InProcessFunctionApp = {
  hostingPlanName: '${resourceNamePrefix}-customv3inprocess-plan'
  functionAppName: '${resourceNamePrefix}-customv3inprocess-func'
  storageName: '${resourceNamePrefix}7customv3inprocess'
  queue: 'customv3inprocess-queue'
  exceptionQueue: 'customv3inprocess-exception-queue'
}
var customV4InProcessFunctionApp = {
  hostingPlanName: '${resourceNamePrefix}-customv4inprocess-plan'
  functionAppName: '${resourceNamePrefix}-customv4inprocess-func'
  storageName: '${resourceNamePrefix}7customv4inprocess'
  queue: 'customv4inprocess-queue'
  exceptionQueue: 'customv4inprocess-exception-queue'
}
var serviceBusNamespace = {
  name: '${resourceNamePrefix}sb'
}

resource applicationInsights_workspaceName 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: applicationInsights.workspaceName
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

resource applicationInsights_name 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsights.name
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: applicationInsights_workspaceName.id
  }
}

resource serviceBusNamespace_name 'Microsoft.ServiceBus/namespaces@2021-11-01' = {
  name: serviceBusNamespace.name
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
  properties: {}
}

resource serviceBusNamespace_name_defaultV3InProcessFunctionApp_queue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespace.name}/${defaultV3InProcessFunctionApp.queue}'
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
    serviceBusNamespace_name
  ]
}

resource serviceBusNamespace_name_defaultV3InProcessFunctionApp_exceptionQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespace.name}/${defaultV3InProcessFunctionApp.exceptionQueue}'
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
    serviceBusNamespace_name
  ]
}

resource serviceBusNamespace_name_defaultV4InProcessFunctionApp_queue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespace.name}/${defaultV4InProcessFunctionApp.queue}'
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
    serviceBusNamespace_name
  ]
}

resource serviceBusNamespace_name_defaultV4InProcessFunctionApp_exceptionQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespace.name}/${defaultV4InProcessFunctionApp.exceptionQueue}'
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
    serviceBusNamespace_name
  ]
}

resource serviceBusNamespace_name_customV3InProcessFunctionApp_queue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespace.name}/${customV3InProcessFunctionApp.queue}'
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
    serviceBusNamespace_name
  ]
}

resource serviceBusNamespace_name_customV3InProcessFunctionApp_exceptionQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespace.name}/${customV3InProcessFunctionApp.exceptionQueue}'
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
    serviceBusNamespace_name
  ]
}

resource serviceBusNamespace_name_customV4InProcessFunctionApp_queue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespace.name}/${customV4InProcessFunctionApp.queue}'
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
    serviceBusNamespace_name
  ]
}

resource serviceBusNamespace_name_customV4InProcessFunctionApp_exceptionQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  name: '${serviceBusNamespace.name}/${customV4InProcessFunctionApp.exceptionQueue}'
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
    serviceBusNamespace_name
  ]
}

resource defaultV3InProcessFunctionApp_storageName 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: defaultV3InProcessFunctionApp.storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
  }
}

resource defaultV3InProcessFunctionApp_hostingPlanName 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: defaultV3InProcessFunctionApp.hostingPlanName
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

resource defaultV3InProcessFunctionApp_functionAppName 'Microsoft.Web/sites@2021-03-01' = {
  name: defaultV3InProcessFunctionApp.functionAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: defaultV3InProcessFunctionApp_hostingPlanName.id
  }
}

resource defaultV3InProcessFunctionApp_functionAppName_appsettings 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${defaultV3InProcessFunctionApp.functionAppName}/appsettings'
  properties: {
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${defaultV3InProcessFunctionApp.storageName};AccountKey=${listKeys(defaultV3InProcessFunctionApp.storageName, '2019-06-01').keys[0].value}'
    APPLICATIONINSIGHTS_CONNECTION_STRING: reference(applicationInsights_name.id, '2020-02-02').ConnectionString
    FUNCTIONS_EXTENSION_VERSION: '~3'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${defaultV3InProcessFunctionApp.storageName};AccountKey=${listKeys(defaultV3InProcessFunctionApp.storageName, '2019-06-01').keys[0].value}'
    WEBSITE_CONTENTSHARE: defaultV3InProcessFunctionApp.functionAppName
    WEBSITE_RUN_FROM_PACKAGE: '1'
    'Secret:ReallySecretValue': reallySecretValue
    ServiceBusConnection: listKeys(resourceId('Microsoft.ServiceBus/namespaces/authorizationRules', serviceBusNamespace.name, 'RootManageSharedAccessKey'), '2017-04-01').primaryConnectionString
  }
  dependsOn: [
    defaultV3InProcessFunctionApp_storageName
    defaultV3InProcessFunctionApp_functionAppName

    serviceBusNamespace_name
  ]
}

resource customV3InProcessFunctionApp_storageName 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: customV3InProcessFunctionApp.storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
  }
}

resource customV3InProcessFunctionApp_hostingPlanName 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: customV3InProcessFunctionApp.hostingPlanName
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

resource customV3InProcessFunctionApp_functionAppName 'Microsoft.Web/sites@2021-03-01' = {
  name: customV3InProcessFunctionApp.functionAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: customV3InProcessFunctionApp_hostingPlanName.id
  }
}

resource customV3InProcessFunctionApp_functionAppName_appsettings 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${customV3InProcessFunctionApp.functionAppName}/appsettings'
  properties: {
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${customV3InProcessFunctionApp.storageName};AccountKey=${listKeys(customV3InProcessFunctionApp.storageName, '2019-06-01').keys[0].value}'
    APPLICATIONINSIGHTS_CONNECTION_STRING: reference(applicationInsights_name.id, '2020-02-02').ConnectionString
    FUNCTIONS_EXTENSION_VERSION: '~3'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${customV3InProcessFunctionApp.storageName};AccountKey=${listKeys(customV3InProcessFunctionApp.storageName, '2019-06-01').keys[0].value}'
    WEBSITE_CONTENTSHARE: customV3InProcessFunctionApp.functionAppName
    WEBSITE_RUN_FROM_PACKAGE: '1'
    'Secret:ReallySecretValue': reallySecretValue
    ServiceBusConnection: listKeys(resourceId('Microsoft.ServiceBus/namespaces/authorizationRules', serviceBusNamespace.name, 'RootManageSharedAccessKey'), '2017-04-01').primaryConnectionString
  }
  dependsOn: [
    customV3InProcessFunctionApp_storageName
    customV3InProcessFunctionApp_functionAppName

    serviceBusNamespace_name
  ]
}

resource defaultV4InProcessFunctionApp_storageName 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: defaultV4InProcessFunctionApp.storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
  }
}

resource defaultV4InProcessFunctionApp_hostingPlanName 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: defaultV4InProcessFunctionApp.hostingPlanName
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

resource defaultV4InProcessFunctionApp_functionAppName 'Microsoft.Web/sites@2021-03-01' = {
  name: defaultV4InProcessFunctionApp.functionAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: defaultV4InProcessFunctionApp_hostingPlanName.id
  }
}

resource defaultV4InProcessFunctionApp_functionAppName_appsettings 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${defaultV4InProcessFunctionApp.functionAppName}/appsettings'
  properties: {
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${defaultV4InProcessFunctionApp.storageName};AccountKey=${listKeys(defaultV4InProcessFunctionApp.storageName, '2021-06-01').keys[0].value}'
    APPLICATIONINSIGHTS_CONNECTION_STRING: reference(applicationInsights_name.id, '2020-02-02').ConnectionString
    FUNCTIONS_EXTENSION_VERSION: '~4'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${defaultV4InProcessFunctionApp.storageName};AccountKey=${listKeys(defaultV4InProcessFunctionApp.storageName, '2021-06-01').keys[0].value}'
    WEBSITE_CONTENTSHARE: defaultV4InProcessFunctionApp.functionAppName
    WEBSITE_RUN_FROM_PACKAGE: '1'
    'Secret:ReallySecretValue': reallySecretValue
    ServiceBusConnection: listKeys(resourceId('Microsoft.ServiceBus/namespaces/authorizationRules', serviceBusNamespace.name, 'RootManageSharedAccessKey'), '2017-04-01').primaryConnectionString
  }
  dependsOn: [
    defaultV4InProcessFunctionApp_storageName
    defaultV4InProcessFunctionApp_functionAppName

    serviceBusNamespace_name
  ]
}

resource customV4InProcessFunctionApp_storageName 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: customV4InProcessFunctionApp.storageName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
  }
}

resource customV4InProcessFunctionApp_hostingPlanName 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: customV4InProcessFunctionApp.hostingPlanName
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

resource customV4InProcessFunctionApp_functionAppName 'Microsoft.Web/sites@2021-03-01' = {
  name: customV4InProcessFunctionApp.functionAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: customV4InProcessFunctionApp_hostingPlanName.id
  }
}

resource customV4InProcessFunctionApp_functionAppName_appsettings 'Microsoft.Web/sites/config@2021-03-01' = {
  name: '${customV4InProcessFunctionApp.functionAppName}/appsettings'
  properties: {
    AzureWebJobsStorage: 'DefaultEndpointsProtocol=https;AccountName=${customV4InProcessFunctionApp.storageName};AccountKey=${listKeys(customV4InProcessFunctionApp.storageName, '2021-06-01').keys[0].value}'
    APPLICATIONINSIGHTS_CONNECTION_STRING: reference(applicationInsights_name.id, '2020-02-02').ConnectionString
    FUNCTIONS_EXTENSION_VERSION: '~4'
    FUNCTIONS_WORKER_RUNTIME: 'dotnet'
    WEBSITE_CONTENTAZUREFILECONNECTIONSTRING: 'DefaultEndpointsProtocol=https;AccountName=${customV4InProcessFunctionApp.storageName};AccountKey=${listKeys(customV4InProcessFunctionApp.storageName, '2021-06-01').keys[0].value}'
    WEBSITE_CONTENTSHARE: customV4InProcessFunctionApp.functionAppName
    WEBSITE_RUN_FROM_PACKAGE: '1'
    'Secret:ReallySecretValue': reallySecretValue
    ServiceBusConnection: listKeys(resourceId('Microsoft.ServiceBus/namespaces/authorizationRules', serviceBusNamespace.name, 'RootManageSharedAccessKey'), '2017-04-01').primaryConnectionString
  }
  dependsOn: [
    customV4InProcessFunctionApp_storageName
    customV4InProcessFunctionApp_functionAppName

    serviceBusNamespace_name
  ]
}

output defaultV3InProcessFunctionAppName string = defaultV3InProcessFunctionApp.functionAppName
output defaultV4InProcessFunctionAppName string = defaultV4InProcessFunctionApp.functionAppName
output customV3InProcessFunctionAppName string = customV3InProcessFunctionApp.functionAppName
output customV4InProcessFunctionAppName string = customV4InProcessFunctionApp.functionAppName
output serviceBusNamespace string = serviceBusNamespace.name
output applicationInsightsName string = applicationInsights.name
