#Requires -Version 7.0
#Requires -Modules Az

<#

.SYNOPSIS
Deploys the testing infrastructure to Azure.

.DESCRIPTION
Provisions the below resources in Azure:

- A Workspace based Application Insights instance
- A Service Bus namespace

The selected subscription is used to deploy.

.PARAMETER Location
An Azure region you can deploy to.

.PARAMETER ResourceNamePrefix
Used to create unique names. For example, with a 'hello' prefix and an Application Insights resource, the resource name will be 'hello-appi'.

.EXAMPLE
.\deploy-testing.ps1 -Location australiaeast -ResourceNamePrefix gabowtesting

.NOTES
You need:

- PowerShell 7 (https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.2)
- Azure PowerShell (https://docs.microsoft.com/en-us/powershell/azure/install-az-ps?view=azps-7.1.0)
- Bicep CLI (https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#install-manually)

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

$resourceGroupName = "$ResourceNamePrefix-rg"

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

$bicepPath = Join-Path $PSScriptRoot 'testing.bicep'

$bicepParameters = @{
    location = $Location
    resourceNamePrefix = $ResourceNamePrefix
}

$deploymentName = "deploy-$((Get-Date).ToString('yyyyMMdd-HHmmss'))-$((New-Guid).Guid.Substring(0, 4))"

$createEnvironmentDeploymentParameters = @{
    Name = $deploymentName
    ResourceGroupName = $resourceGroupName
    TemplateFile = $bicepPath
    TemplateParameterObject = $bicepParameters
    SkipTemplateParameterPrompt = $true
    Force = $true
}

New-AzResourceGroupDeployment @createEnvironmentDeploymentParameters | Out-Null
