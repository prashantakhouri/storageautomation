# - 
# - Creates Azure Function App and required resources
# - 

# Create an Serverless azure app service plan
resource "azurerm_app_service_plan" "this" {
  name                                = var.appserviceplan_name
  location                            = var.location
  resource_group_name                 = var.resource_group_name
  kind                                = "FunctionApp" //"FunctionApp"
  tags                                = var.tags
  sku {
    tier                              = var.tier        //"Dynamic","Standard"
    size                              = var.size              //"Y1","S1"
  }
}

# - Get the App Insight Name to assign to Function App

data "azurerm_application_insights" "this" {
  name                                 = var.appinsights_name
  resource_group_name                  = var.resource_group_name
}

# - Get the Storage account details to assign to Function App 
data "azurerm_storage_account" "this" {
  name                                 = var.storage_account_name
  resource_group_name                  = var.resource_group_name
}
# - Function App for App Service Plan
resource "azurerm_function_app" "this" {
  name                                 = var.name
  location                             = var.location
  resource_group_name                  = var.resource_group_name
  app_service_plan_id                  = azurerm_app_service_plan.this.id
  storage_account_name                 = data.azurerm_storage_account.this.name
  storage_account_access_key           = data.azurerm_storage_account.this.primary_access_key
  tags                                 = var.tags
  version                              = "~3"
  identity {
    type = "SystemAssigned"   
  }
  app_settings = {
        FUNCTIONS_WORKER_RUNTIME       = var.FUNCTIONS_WORKER_RUNTIME
        WEBSITE_NODE_DEFAULT_VERSION   = "~3" 
        AppInsights_InstrumentationKey = data.azurerm_application_insights.this.instrumentation_key   
    }
  # auth_settings {
  #   enabled = true
  #   microsoft {
  #     client_id = "821f4a46-3105-4ae5-bfb2-18aa61b13113"
  #     client_secret = "AArm42.ZbWo~Wl4Rb_gC9hNJ_PY1.2k6MH"
  #   }
  #   default_provider = "AzureActiveDirectory"
  # }
  site_config {
    cors {
      allowed_origins = ["*"]
    }
      app_scale_limit                   = var.app_scale_limit
  }
}

# resource "azurerm_monitor_autoscale_setting" "this" {
#   name                                 = "myAutoscaleSetting"
#   resource_group_name                  = var.resource_group_name
#   location                             = var.location
#   target_resource_id                   = azurerm_function_app.this.id

#   profile {
#     name = "defaultProfile"

#     capacity {
#       default = 1
#       minimum = 1
#       maximum = 8
#     }
#   }
# }

# - Get the Key Vault name to assign Function App to the access policy
data "azurerm_key_vault" "this" {
  name                                 = var.key_vault_name
  resource_group_name                  = var.resource_group_name
}
# - Access policy Creation for Function App
resource "azurerm_key_vault_access_policy" "this" {
  key_vault_id                         = data.azurerm_key_vault.this.id
  tenant_id                            = azurerm_function_app.this.identity[0].tenant_id
  object_id                            = azurerm_function_app.this.identity[0].principal_id

  key_permissions = [
    "Get","List"
  ]

  secret_permissions = [
    "Get", "List"
  ]
}
# - Get the Key Vault name to assign Function App to the access policy
data "azurerm_key_vault" "this1" {
  provider                             = azurerm.kv
  name                                 = var.kv_name
  resource_group_name                  = var.rg_name
}
resource "azurerm_key_vault_access_policy" "this1" {
  provider                             = azurerm.kv
  key_vault_id                         = data.azurerm_key_vault.this1.id
  tenant_id                            = azurerm_function_app.this.identity[0].tenant_id
  object_id                            = azurerm_function_app.this.identity[0].principal_id

  key_permissions = [
    "Get","List"
  ]

  secret_permissions = [
    "Get", "List"
  ]
  depends_on = [
    azurerm_function_app.this, azurerm_key_vault_access_policy.this
  ]
}
# - Add Key Vault Secrets
 resource "azurerm_key_vault_secret" "this" {
   count                     = var.kvsecret_enable ? 1 : 0
   name                      = var.secret_name 
   value                     = "https://${azurerm_function_app.this.name}.azurewebsites.net"
   key_vault_id              = data.azurerm_key_vault.this.id
   tags                      = var.tags
 }

