#Requires -Version 7.0
#Requires -Modules Az

<#

.SYNOPSIS
Deploys the project to Azure.

.DESCRIPTION
Provisions the below resources in Azure:

- An Application Insights instance
- A Service Bus namespace
- Two Function Apps and their supporting storage accounts

The selected subscription is used to deploy.

.PARAMETER Location
An Azure region you can deploy to.

.PARAMETER ResourceNamePrefix
Used to create unique names. For example, with a 'hello' prefix and an Application Insights resource, the resource name will be 'hello-appi'.

.EXAMPLE
.\deploy.ps1 -Location australiaeast -ResourceNamePrefix gabow

.NOTES
You need:

- PowerShell 7 (https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-windows?view=powershell-7)
- Azure PowerShell (https://docs.microsoft.com/en-us/powershell/azure/install-az-ps?view=azps-5.2.0)

Select the subscription you want to deploy to before running this script.

.LINK
https://github.com/gabrielweyer/azure-functions-limitations/blob/main/README.md

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
    $sourcePath = Join-Path $PSScriptRoot 'src' $FunctionName

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

Write-Host 'Publishing Default API to file system'

$defaultApiArchivePath = Publish-FunctionApp 'DefaultApi'

Write-Verbose "Published Default API to '$defaultApiArchivePath'"

Write-Host 'Publishing Custom API to file system'

$customApiArchivePath = Publish-FunctionApp 'CustomApi'

Write-Verbose "Published Custom API to '$customApiArchivePath'"

Write-Host 'Creating resource group'

$createResourceGroupParameters = @{
    Name = $resourceGroupName
    Location = $Location
    Force = $true
}

New-AzResourceGroup @createResourceGroupParameters | Out-Null

Write-Verbose "Created (or updated) resource group '$resourceGroupName'"

Write-Host 'Deploying ARM template (takes a while)'

$templatePath = Join-Path $PSScriptRoot 'template.json'

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

$defaultApiFunctionAppName = $armDeploymentResult.Outputs.Item('defaultApiFunctionAppName').Value
$customApiFunctionAppName = $armDeploymentResult.Outputs.Item('customApiFunctionAppName').Value
$serviceBusNamesapce = $armDeploymentResult.Outputs.Item('serviceBusNamespace').Value

Write-Verbose "Default API Function App name is '$defaultApiFunctionAppName'"
Write-Verbose "Custom API Function App name is '$customApiFunctionAppName'"
Write-Verbose "Service Bus namespace is '$serviceBusNamesapce'"

Write-Host 'Deploying Default API to Azure'

$publishDefaultApiParameters = @{
    ResourceGroupName = $resourceGroupName
    Name = $defaultApiFunctionAppName
    ArchivePath = $defaultApiArchivePath
    Force = $true
}

Publish-AzWebapp @publishDefaultApiParameters | Out-Null

Write-Host 'Deploying Custom API to Azure'

$publishCustomApiParameters = @{
    ResourceGroupName = $resourceGroupName
    Name = $customApiFunctionAppName
    ArchivePath = $customApiArchivePath
    Force = $true
}

Publish-AzWebapp @publishCustomApiParameters | Out-Null

Write-Host 'Setting local user secrets'

dotnet user-secrets set Secret:ReallySecretValue "$secretValue" --id 074ca336-270b-4832-9a1a-60baf152b727 | Out-Null
Write-verbose "Set secret 'Secret:ReallySecretValue'"

$connectionStrings = Get-AzServiceBusKey -ResourceGroup $resourceGroupName -Namespace $serviceBusNamesapce -Name RootManageSharedAccessKey
dotnet user-secrets set ServiceBusConnection "$($connectionStrings.PrimaryConnectionString)" --id 074ca336-270b-4832-9a1a-60baf152b727 | Out-Null
Write-verbose "Set secret 'ServiceBusConnection'"
