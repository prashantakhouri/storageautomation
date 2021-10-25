# - 
# - Creates Azure Key Vault and assign mandatory tags
# - 

data "azurerm_key_vault" "this" {
  name                       = var.key_vault_name
  resource_group_name        = var.resource_group_name
}
data "azurerm_key_vault" "this1" {
  provider                   = azurerm.kv
  name                       = var.kv_name
  resource_group_name        = var.rg_name
}

data "azurerm_api_management" "this" {
  name                       = var.apim_name
  resource_group_name        = var.resource_group_name
}

resource "azurerm_key_vault_secret" "this" {
  name                      = var.secret_name 
  value                     = "https://${data.azurerm_key_vault.this1.name}.vault.azure.net"
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }

resource "azurerm_key_vault_secret" "this1" {
  name                      = var.secret_name1 
  value                     = var.value1
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }

resource "azurerm_key_vault_secret" "this2" {
  name                      = var.secret_name2
  value                     = var.value2
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }

resource "azurerm_key_vault_secret" "this3" {
  name                      = var.secret_name3
  value                     = var.value3
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }

resource "azurerm_key_vault_secret" "this4" {
  name                      = var.secret_name4
  value                     = var.value4
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }

resource "azurerm_key_vault_secret" "this5" {
  name                      = var.secret_name5
  value                     = var.value5
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }

resource "azurerm_key_vault_secret" "this6" {
  name                      = var.secret_name6
  value                     = var.value6
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }

resource "azurerm_key_vault_secret" "this7" {
  name                      = var.secret_name7
  value                     = var.value7
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }

resource "azurerm_key_vault_secret" "this8" {
  name                      = var.secret_name8
  value                     = var.value8
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }

resource "azurerm_key_vault_secret" "this9" {
  name                      = var.secret_name9
  value                     = var.value9
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }
resource "azurerm_key_vault_secret" "this10" {
  name                      = var.secret_name10
  value                     = var.value10
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }
resource "azurerm_key_vault_secret" "this11" {
  name                      = var.secret_name11
  value                     = "https://${data.azurerm_api_management.this.name}.azure-api.net/productionstore/getfunctionstatus/"
  key_vault_id              = data.azurerm_key_vault.this.id
  tags                      = var.tags
 }