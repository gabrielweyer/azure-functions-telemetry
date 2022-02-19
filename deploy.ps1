#Requires -Version 7.0
#Requires -Modules Az

<#

.SYNOPSIS
Deploys the project to Azure.

.DESCRIPTION
Provisions the below resources in Azure:

- A Workspace based Application Insights instance
- A Service Bus namespace
- Four Function Apps and their supporting storage accounts

The selected subscription is used to deploy.

.PARAMETER Location
An Azure region you can deploy to.

.PARAMETER ResourceNamePrefix
Used to create unique names. For example, with a 'hello' prefix and an Application Insights resource, the resource name will be 'hello-appi'.

.EXAMPLE
.\deploy.ps1 -Location australiaeast -ResourceNamePrefix gabow

.NOTES
You need:

- PowerShell 7 (https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.2)
- Azure PowerShell (https://docs.microsoft.com/en-us/powershell/azure/install-az-ps?view=azps-7.1.0)

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

function Publish-FunctionApp {
    param(
        [Parameter()]
        [string]$FunctionName
    )

    $outputPath = Join-Path $PSScriptRoot 'out' $FunctionName
    $sourcePath = Join-Path $PSScriptRoot 'samples' $FunctionName

    dotnet clean --configuration Release --output $outputPath $sourcePath | Out-Null

    dotnet publish --configuration Release --output $outputPath $sourcePath | Out-Null

    $archivePath = Join-Path $outputPath "$FunctionName.zip"

    $compressParameters = @{
        Path = Join-Path $outputPath '*'
        CompressionLevel = 'Fastest'
        DestinationPath = $archivePath
    }
    Compress-Archive @compressParameters

    $archivePath
}

$resourceGroupName = "$ResourceNamePrefix-rg"
$secretValue = 'TopSecret'

Write-Host "Ensuring that you're signed-in into your Azure account"

$selectedSubscription = Get-AzContext | Select-Object -ExpandProperty Subscription

if ($null -eq $selectedSubscription) {
    throw 'You need to be signed-in into your Azure account to run this script'
} else {
    Write-Verbose "Deploying to subscription '$($selectedSubscription.Id) ($($selectedSubscription.Name))'"
}

Write-Host 'Publishing Default V3 In-Process Function App to file system'

$defaultV3InProcessFunctionArchivePath = Publish-FunctionApp 'DefaultV3InProcessFunction'

Write-Verbose "Published Default V3 In-Process Function App to '$defaultV3InProcessFunctionArchivePath'"

Write-Host 'Publishing Default V4 In-Process Function App to file system'

$defaultV4InProcessFunctionArchivePath = Publish-FunctionApp 'DefaultV4InProcessFunction'

Write-Verbose "Published Default V4 In-Process Function App to '$defaultV4InProcessFunctionArchivePath'"

Write-Host 'Publishing Custom V3 In-Process Function App to file system'

$customV3InProcessFunctionArchivePath = Publish-FunctionApp 'CustomV3InProcessFunction'

Write-Verbose "Published Custom V3 In-Process Function App to '$customV3InProcessFunctionArchivePath'"

Write-Host 'Publishing Custom V4 In-Process Function App to file system'

$customV4InProcessFunctionArchivePath = Publish-FunctionApp 'CustomV4InProcessFunction'

Write-Verbose "Published Custom V4 In-Process Function App to '$customV4InProcessFunctionArchivePath'"

Write-Host 'Creating (or updating) resource group'

$createResourceGroupParameters = @{
    Name = $resourceGroupName
    Location = $Location
    Force = $true
}

New-AzResourceGroup @createResourceGroupParameters | Out-Null

Write-Verbose "Created (or updated) resource group '$resourceGroupName'"

Write-Host 'Deploying ARM template (takes a while)'

$templatePath = Join-Path $PSScriptRoot 'deploy' 'template.json'

$templateParameters = @{
    location = $Location
    resourceNamePrefix = $ResourceNamePrefix
    reallySecretValue = $secretValue
}

$createEnvironmentDeploymentParameters = @{
    Name = "deploy-$((Get-Date).ToString('yyyyMMdd-HHmmss'))-$((New-Guid).Guid.Substring(0, 4))"
    ResourceGroupName = $resourceGroupName
    TemplateFile = $templatePath
    TemplateParameterObject = $templateParameters
    SkipTemplateParameterPrompt = $true
    Force = $true
}

$armDeploymentResult = New-AzResourceGroupDeployment @createEnvironmentDeploymentParameters

$defaultV3InProcessFunctionAppName = $armDeploymentResult.Outputs.Item('defaultV3InProcessFunctionAppName').Value
$defaultV4InProcessFunctionAppName = $armDeploymentResult.Outputs.Item('defaultV4InProcessFunctionAppName').Value
$customV3InProcessFunctionAppName = $armDeploymentResult.Outputs.Item('customV3InProcessFunctionAppName').Value
$customV4InProcessFunctionAppName = $armDeploymentResult.Outputs.Item('customV4InProcessFunctionAppName').Value
$serviceBusNamespace = $armDeploymentResult.Outputs.Item('serviceBusNamespace').Value
$applicationInsightsName = $armDeploymentResult.Outputs.Item('applicationInsightsName').Value

Write-Verbose "Default V3 In-Process Function App name is '$defaultV3InProcessFunctionAppName'"
Write-Verbose "Default V4 In-Process Function App name is '$defaultV4InProcessFunctionAppName'"
Write-Verbose "Custom V3 In-Process Function App name is '$customV3InProcessFunctionAppName'"
Write-Verbose "Custom V4 In-Process Function App name is '$customV4InProcessFunctionAppName'"
Write-Verbose "Service Bus namespace is '$serviceBusNamespace'"
Write-Verbose "Application Insights instance is '$applicationInsightsName'"

Write-Host 'Deploying Default V3 In-Process Function App to Azure'

$publishDefaultV3InProcessParameters = @{
    ResourceGroupName = $resourceGroupName
    Name = $defaultV3InProcessFunctionAppName
    ArchivePath = $defaultV3InProcessFunctionArchivePath
    Force = $true
}

Publish-AzWebapp @publishDefaultV3InProcessParameters | Out-Null

Write-Host 'Deploying Default V4 In-Process Function App to Azure'

$publishDefaultV4InProcessParameters = @{
    ResourceGroupName = $resourceGroupName
    Name = $defaultV4InProcessFunctionAppName
    ArchivePath = $defaultV4InProcessFunctionArchivePath
    Force = $true
}

Publish-AzWebapp @publishDefaultV4InProcessParameters | Out-Null

Write-Host 'Deploying Custom V3 In-Process Function App to Azure'

$publishCustomV3InProcessParameters = @{
    ResourceGroupName = $resourceGroupName
    Name = $customV3InProcessFunctionAppName
    ArchivePath = $customV3InProcessFunctionArchivePath
    Force = $true
}

Publish-AzWebapp @publishCustomV3InProcessParameters | Out-Null

Write-Host 'Deploying Custom V4 In-Process Function App to Azure'

$publishCustomV4InProcessParameters = @{
    ResourceGroupName = $resourceGroupName
    Name = $customV4InProcessFunctionAppName
    ArchivePath = $customV4InProcessFunctionArchivePath
    Force = $true
}

Publish-AzWebapp @publishCustomV4InProcessParameters | Out-Null

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
