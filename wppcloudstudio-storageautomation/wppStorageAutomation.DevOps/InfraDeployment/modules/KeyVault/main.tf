# - 
# - Creates Azure Key Vault and assign mandatory tags
# - 
data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "this" {
  name                            = var.name
  resource_group_name             = var.resource_group_name
  location                        = var.location
  sku_name                        = var.sku_name  //"Standard"
  tenant_id                       = data.azurerm_client_config.current.tenant_id
  enabled_for_disk_encryption     = var.enabled_for_disk_encryption       //"true"
  enabled_for_template_deployment = var.enabled_for_template_deployment   //"true"
  enabled_for_deployment          = var.enabled_for_deployment            //"true"
  tags                            = var.tags
}
resource "azurerm_key_vault_access_policy" "this" {
  key_vault_id                    = azurerm_key_vault.this.id
  tenant_id                       = data.azurerm_client_config.current.tenant_id
  object_id                       = data.azurerm_client_config.current.object_id

  key_permissions = [
      "get", "list" , "create", "delete", "update"
    ]
  secret_permissions = [
      "get", "list" , "set", "delete", "recover", "backup", "restore", "purge"
    ]
}

