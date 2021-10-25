# -
# - Outputs of the module :  KeyVault
# -

output "key_vault_id" {  
    description = "id of the key vault provisioned"  
    value = "${azurerm_key_vault.this.id}"  
}  
output "name" {  
    description = "name of the key vault provisioned"  
    value = "${azurerm_key_vault.this.name}"  
}  
output "location" {  
    description = "location of the key vault provisioned"  
    value = "${azurerm_key_vault.this.location}"  
}
