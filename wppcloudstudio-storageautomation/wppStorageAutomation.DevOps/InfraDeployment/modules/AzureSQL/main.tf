# -
# - Create Azure SQL Single DB and assign mandatory tags
# - 

resource "azurerm_mssql_server" "this" {
  name                         = var.name
  location                     = var.location
  resource_group_name          = var.resource_group_name
  version                      = "12.0"
  administrator_login          = var.administrator_login
  administrator_login_password = var.administrator_login_password
  minimum_tls_version          = "1.2"
  tags                         = var.tags
    identity {
    type = "SystemAssigned"   
  }
}

resource "azurerm_mssql_database" "this" {
  name                         = var.sqldatabase_name
  tags                         = var.tags
  server_id                    = azurerm_mssql_server.this.id
  collation                    = "SQL_Latin1_General_CP1_CI_AS"
  #license_type                 = "LicenseIncluded"
  #max_size_gb                  = 4
  #read_scale                   = true
  #sku_name                     = "GP_S_Gen5_2"
  #zone_redundant               = true
}

resource "azurerm_mssql_firewall_rule" "this" {
  name             = var.firewall_rulenames
  server_id        = azurerm_mssql_server.this.id
  start_ip_address = var.start_ip_address //"0.0.0.0"
  end_ip_address   = var.end_ip_address //"0.0.0.0"
}

data "azurerm_key_vault" "this" {
  name                = var.key_vault_name
  resource_group_name = var.resource_group_name
}
# - Add Key Vault Secrets
 resource "azurerm_key_vault_secret" "this" {
  name                      = var.secret_name
  #value                     = "${azurerm_mssql_server.this.administrator_login_password}"
  value                     = var.value
  key_vault_id              = data.azurerm_key_vault.this.id     //azurerm_key_vault.this.id
  tags                      = var.tags
 
 }

resource "azurerm_key_vault_access_policy" "this" {
  key_vault_id = data.azurerm_key_vault.this.id
  tenant_id    = azurerm_mssql_server.this.identity[0].tenant_id
  object_id    = azurerm_mssql_server.this.identity[0].principal_id

  key_permissions = [
    "Get","List", "delete"
  ]

  secret_permissions = [
    "Get", "List", "set", "delete"
  ]
}