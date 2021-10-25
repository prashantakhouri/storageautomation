# -
# - Creates storage account and assign mandatory tags
# -
resource "azurerm_storage_account" "this" {
  name                      = var.name
  resource_group_name       = var.resource_group_name
  location                  = var.location
  account_tier              = var.account_tier
  account_replication_type  = var.account_replication_type    //"LRS""ZGRS"
  account_kind              = var.account_kind
  tags                      = var.tags
}

# - Create Container
# resource "azurerm_storage_container" "this" {
#   count                     = var.container_enable ? 1 : 0
#   name                      = var.container_name
#   storage_account_name      = azurerm_storage_account.this.name
#   container_access_type     = var.container_access_type    //"private"
# }

# # - Create File share
# resource "azurerm_storage_share" "this" {
#   count                     = var.fileshare_enable ? 1 : 0
#   name                      = var.fileshare_name
#   storage_account_name      = azurerm_storage_account.this.name
#   quota                     = var.quota //50
#   depends_on  = [
#     azurerm_storage_account.this
#   ]     
# }

# - Create Queue
resource "azurerm_storage_queue" "this" {
  count                = var.queue_enable ? 1 : 0
  name                 = var.queue_name
  storage_account_name = azurerm_storage_account.this.name
}

# - Get Keyvault details
data "azurerm_key_vault" "this" {
  name                      = var.key_vault_name
  resource_group_name       = var.resource_group_name
}

# - Add Key Vault Secrets

resource "azurerm_key_vault_secret" "this" {
   count                     = var.kvsecret_enable ? 1 : 0
   name                      = var.secret_name
   value                     = "${azurerm_storage_account.this.primary_connection_string}"
   key_vault_id              = data.azurerm_key_vault.this.id
   tags                      = var.tags
 }

resource "azurerm_key_vault_secret" "this1" {
   count                     = var.queuekvsecret_enable ? 1 : 0
   name                      = var.queuesecret_name
   value                     = var.queuesecret_value
   key_vault_id              = data.azurerm_key_vault.this.id
   tags                      = var.tags
 }