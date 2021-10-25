# -
# - Outputs of the module :  KeyVault
# -

output "key_vault_id" {  
    description = "id of the key vault provisioned"  
    value = "${data.azurerm_key_vault.this.id}"  
}  
output "name" {  
    description = "name of the key vault provisioned"  
    value = "${data.azurerm_key_vault.this.name}"  
}  
output "secret_name" {  
    description = "location of the key vault provisioned"  
    value = "${azurerm_key_vault_secret.this.name}"  
}
