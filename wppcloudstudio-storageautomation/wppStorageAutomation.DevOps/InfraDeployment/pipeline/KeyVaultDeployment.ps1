##Powershell Script:-
Connect-AzAccount
az account set --subscription "wpp-wppcs-csapp-p-01"

#Below resources name to be modified as required.
$RG_NAME = "rg-wppcs-neu-p-devops-01"

$KV_NAME = "kv-wppcs-neu-p-devops-01"

$LOCATION = "North Europe"

##Create RG
New-AzResourceGroup -Name $RG_NAME -Location $LOCATION
##Create KV
New-AzKeyVault -VaultName $KV_NAME -ResourceGroupName $RG_NAME -Location $LOCATION


##Set the secrets in the Vault
##Okta Client ID and URL - Need to be provided by Robert
#$Secret = ConvertTo-SecureString -String 'Password' -AsPlainText -Force
$secret1 = ConvertTo-SecureString " " -AsPlainText -Force
$secret2 = ConvertTo-SecureString " " -AsPlainText -Force
$secret3 = ConvertTo-SecureString " " -AsPlainText -Force
$secret4 = ConvertTo-SecureString " " -AsPlainText -Force
$secret5 = ConvertTo-SecureString " " -AsPlainText -Force
$secret6 = ConvertTo-SecureString " " -AsPlainText -Force
$secret7 = ConvertTo-SecureString " " -AsPlainText -Force
$secret8 = ConvertTo-SecureString " " -AsPlainText -Force
$secret9 = ConvertTo-SecureString " " -AsPlainText -Force
$secret10 = ConvertTo-SecureString " " -AsPlainText -Force


Set-AzKeyVaultSecret -VaultName $KV_NAME -Name "authserver-app-clientid" -SecretValue $secret1
Set-AzKeyVaultSecret -VaultName $KV_NAME -Name "authserver-app-url" -SecretValue $secret2

##SDDL values from Developer Team


Set-AzKeyVaultSecret -VaultName $KV_NAME -Name "sddl-create-production" -SecretValue $secret3
Set-AzKeyVaultSecret -VaultName $KV_NAME -Name "sddl-readonly-makeoffline" -SecretValue $secret4

##SQL Admin ID and Passowrd - For Prod Password should be given by WPP


Set-AzKeyVaultSecret -VaultName $KV_NAME -Name "sqlAdminLoginId"-SecretValue $secret5
Set-AzKeyVaultSecret -VaultName $KV_NAME -Name "sqlAdminLoginPassword" -SecretValue $secret6

##Sub ID where the KeyVault containing the secrets od SA created by Creative Desktop

Set-AzKeyVaultSecret -VaultName $KV_NAME -Name "tf-subscription-hub-id" -SecretValue $secret7

##Sub ID where our Infrastructure will be deployed

Set-AzKeyVaultSecret -VaultName $KV_NAME -Name "tf-subscription-id" -SecretValue $secret8

##SDDL values from Developer Team

Set-AzKeyVaultSecret -VaultName $KV_NAME -Name "sddl-fullcontrol-config" -SecretValue $secret9

##Subscription ID of the Storage Accounts created by CD Team.

Set-AzKeyVaultSecret -VaultName $KV_NAME -Name "tf-subscription-sa-id" -SecretValue $secret10

