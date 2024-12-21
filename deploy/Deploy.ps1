#Requires -Version 7.0
#Requires -Modules Az.Accounts, Az.Resources, Az.Websites

<#

.SYNOPSIS
Deploys the sample Functions to Azure.

.DESCRIPTION
Provisions the below resources in Azure:

- A Workspace based Application Insights instance
- A Service Bus namespace
- Three Function Apps and their supporting storage accounts

Please run `build.ps1 --package` beforehand to publish the Functions to file system.

The selected subscription is used to deploy.

.PARAMETER Location
An Azure region you can deploy to.

.PARAMETER ResourceNamePrefix
Used to create unique names. For example, with a 'hello' prefix and an Application Insights resource, the resource name will be 'hello-appi'.

.EXAMPLE
.\Deploy.ps1 -Location australiaeast -ResourceNamePrefix gabow

.NOTES
You need:

- PowerShell 7 (https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.4)
- Azure PowerShell (https://learn.microsoft.com/en-us/powershell/azure/install-azure-powershell)
- Bicep CLI (https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#install-manually)

Select the subscription you want to deploy to before running this script.

.LINK
https://github.com/gabrielweyer/azure-functions-telemetry/blob/main/README.md

#>

[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)]
    [string]$Location,

    [Parameter(Mandatory=$true)]
    [string]$ResourceNamePrefix
)

$ErrorActionPreference = 'Stop'

$outputPath = Join-Path $PSScriptRoot '..' 'artifacts' 'out' 'functions'

$resourceGroupName = "$ResourceNamePrefix-rg"
$secretValue = 'TopSecret'

Write-Host "Ensuring that you're signed-in into your Azure account"

$selectedSubscription = Get-AzContext | Select-Object -ExpandProperty Subscription

if ($null -eq $selectedSubscription) {
    throw 'You need to be signed-in into your Azure account to run this script'
} else {
    Write-Verbose "Deploying to subscription '$($selectedSubscription.Id) ($($selectedSubscription.Name))'"
}

Write-Host 'Creating (or updating) resource group'

$createResourceGroupParameters = @{
    Name = $resourceGroupName
    Location = $Location
    Force = $true
}

New-AzResourceGroup @createResourceGroupParameters | Out-Null

Write-Verbose "Created (or updated) resource group '$resourceGroupName'"

Write-Host 'Deploying Bicep file (takes a while)'

$bicepPath = Join-Path $PSScriptRoot 'main.bicep'

$deploymentNameSuffix = "$((Get-Date).ToString('yyyyMMdd-HHmmss'))-$((New-Guid).Guid.Substring(0, 4))"

$bicepParameters = @{
    location = $Location
    resourceNamePrefix = $ResourceNamePrefix
    reallySecretValue = $secretValue
    deploymentNameSuffix = $deploymentNameSuffix
}

$createEnvironmentDeploymentParameters = @{
    Name = "deploy-$deploymentNameSuffix"
    ResourceGroupName = $resourceGroupName
    TemplateFile = $bicepPath
    TemplateParameterObject = $bicepParameters
    SkipTemplateParameterPrompt = $true
    Force = $true
}

$deploymentResult = New-AzResourceGroupDeployment @createEnvironmentDeploymentParameters

$defaultV4InProcessFunctionAppName = $deploymentResult.Outputs.Item('defaultV4InProcessFunctionAppName').Value
$defaultV4IsolatedFunctionAppName = $deploymentResult.Outputs.Item('defaultV4IsolatedFunctionAppName').Value
$customV4InProcessFunctionAppName = $deploymentResult.Outputs.Item('customV4InProcessFunctionAppName').Value
$serviceBusNamespace = $deploymentResult.Outputs.Item('serviceBusNamespace').Value
$applicationInsightsName = $deploymentResult.Outputs.Item('applicationInsightsName').Value

Write-Verbose "Default V4 In-Process Function App name is '$defaultV4InProcessFunctionAppName'"
Write-Verbose "Default V4 Isolated Function App name is '$defaultV4IsolatedFunctionAppName'"
Write-Verbose "Custom V4 In-Process Function App name is '$customV4InProcessFunctionAppName'"
Write-Verbose "Service Bus namespace is '$serviceBusNamespace'"
Write-Verbose "Application Insights instance is '$applicationInsightsName'"

Write-Host 'Deploying Default V4 In-Process Function App to Azure'

$defaultV4InProcessFunctionArchivePath = Join-Path $outputPath 'DefaultV4InProcessFunction.zip'

$publishDefaultV4InProcessParameters = @{
    ResourceGroupName = $resourceGroupName
    Name = $defaultV4InProcessFunctionAppName
    ArchivePath = $defaultV4InProcessFunctionArchivePath
    Force = $true
}

Publish-AzWebapp @publishDefaultV4InProcessParameters | Out-Null

Write-Host 'Deploying Default V4 Isolated Function App to Azure'

$defaultV4IsolatedFunctionArchivePath = Join-Path $outputPath 'DefaultV4IsolatedFunction.zip'

$publishDefaultV4IsolatedParameters = @{
    ResourceGroupName = $resourceGroupName
    Name = $defaultV4IsolatedFunctionAppName
    ArchivePath = $defaultV4IsolatedFunctionArchivePath
    Force = $true
}

Publish-AzWebapp @publishDefaultV4IsolatedParameters | Out-Null

Write-Host 'Deploying Custom V4 In-Process Function App to Azure'

$customV4InProcessFunctionArchivePath = Join-Path $outputPath 'CustomV4InProcessFunction.zip'

$publishCustomV4InProcessParameters = @{
    ResourceGroupName = $resourceGroupName
    Name = $customV4InProcessFunctionAppName
    ArchivePath = $customV4InProcessFunctionArchivePath
    Force = $true
}

Publish-AzWebapp @publishCustomV4InProcessParameters | Out-Null

if ($null -eq $Env:GITHUB_ACTIONS) {
    Write-Host 'Setting local user secrets'

    $userSecretsId = '074ca336-270b-4832-9a1a-60baf152b727'

    dotnet user-secrets set Secret:ReallySecretValue "$secretValue" --id $userSecretsId | Out-Null
    Write-verbose "Set secret 'Secret:ReallySecretValue'"

    $serviceBusConnectionStrings = Get-AzServiceBusKey -ResourceGroup $resourceGroupName -Namespace $serviceBusNamespace -Name RootManageSharedAccessKey
    dotnet user-secrets set ServiceBusConnection "$($serviceBusConnectionStrings.PrimaryConnectionString)" --id $userSecretsId | Out-Null
    Write-verbose "Set secret 'ServiceBusConnection'"

    $applicationInsightsResource = Get-AzApplicationInsights -ResourceGroupName $resourceGroupName -Name $applicationInsightsName
    dotnet user-secrets set APPLICATIONINSIGHTS_CONNECTION_STRING "$($applicationInsightsResource.ConnectionString)" --id $userSecretsId | Out-Null
    Write-verbose "Set secret 'APPLICATIONINSIGHTS_CONNECTION_STRING'"
} else {
    Write-Host 'Skipping setting local user secrets as we''re running in GitHub Actions'
}
