<#
.SYNOPSIS
    Populate Data as part of Pre-Requisites for Performance Testing	
	
.DESCRIPTION
	This script contains following:
    1) Creation of 'Production Store' leveraging an existing Azure Storage Account
    2) Creation of 'Production(s)' within a 'Production Store'
    3) Copy Sample Video Production (Small: 1GB, Medium: 5GB, Large: 25GB) from an existing Production to this new Production Store (refer #1)

		
.NOTES
	*Author  : Sumit Dua
	*Version : 1.0
#>

#PS Script
try
{
    #Parameter Declaration
    param(
          [string]  $storageAccountKey = [string]::Empty
         )

    Write-Host 'Create Production'
    Write-Host 'Printing Variables...'
    Write-Host '$(tf-subscription-id)'
    Write-Host '$(storageAccountName)'
    Write-Host '$(resourceGroupName)'
    az login --service-principal -u  $(wpp-it-cloudstudio-app-d-001-id) -p $(wpp-it-cloudstudio-app-d-001-sec) --tenant $(wpp-it-cloudstudio-tenant) 
    az account set --subscription $(tf-subscription-id) 
    export storageAccountKey="$(az storage account keys list -g $(resourceGroupName) -n $(storageAccountName) --query '[0].value' -o tsv)"
    az storage directory create --account-name '$(storageAccountName)' --account-key "$storageAccountKey" --share-name "perf-ps" --name "PerfTestProduction05" --output none
    Write-Host 'Production created successfully'
}
catch
{
    Write-Host "Exception Message: $($_.Exception.Message)"
}