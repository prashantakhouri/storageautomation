# -
# - Outputs of the module :  Storage Account
# -

output "id" {  
    description = "id of the storage account provisioned"  
    value = "${azurerm_storage_account.this.id}"  
}  
output "name" {  
    description = "name of the storage account provisioned"  
    value = "${azurerm_storage_account.this.name}"  
}  

output "location" {  
    description = "location of the storage account provisioned"  
    value = "${azurerm_storage_account.this.location}"  
}
output "sas_token" {
  description = "primary access key of the storage account provisioned"
  value = azurerm_storage_account.this.primary_access_key 
  sensitive   = true
}
output "connection_string_primary" {
  value = "${azurerm_storage_account.this.primary_connection_string}"
}

