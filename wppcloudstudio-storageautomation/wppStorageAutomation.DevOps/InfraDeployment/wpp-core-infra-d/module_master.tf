# -
# - Master file : all the modules for core infrastructure are called here
# -

# - Calling ResourceGroup module to create resource group
  module "resource_group" {
    source   = "../modules/ResourceGroup"
    location = var.rg_location
    name     = "rg-${var.project_code}-${var.locationshortprefix}-${var.Environment}-csportal-${var.order}"  //rg-wppcs-neu-d-csportal-01
    tags     = var.rg_tags
  }
   
# - Calling StorageAccount module to create an azure storage account

  module "storage_account" {
    source = "../modules/StorageAccount"
    for_each                 = var.storage_accounts
    name                     = "sa${var.project_code}${var.locationshortprefix}${var.Environment}${each.value["storage_account_purpose"]}${var.tiershortprefix}${var.order}"  //sawppcsneutfncstand01 
    resource_group_name      = module.resource_group.name
    location                 = module.resource_group.location
    account_tier             = each.value["storage_account_tier"]
    account_replication_type = each.value["storage_account_repl_type"]
    account_kind             = each.value["storage_account_account_kind"]
    kvsecret_enable          = each.value["storage_account_kvsecret_enable"]
    secret_name              = "kv-sa-sa${var.project_code}${var.locationshortprefix}${var.Environment}${each.value["storage_account_purpose"]}${var.tiershortprefix}${var.order}" // kv-sa-sawppcsneutfncstand01
    queue_enable             = each.value["storage_account_queue_enable"]
    queue_name               = each.value["storage_account_queue_name"]
    queuekvsecret_enable     = each.value["storage_account_queuekvsecret_enable"]
    queuesecret_name         = each.value["storage_account_queuesecret_name"]
    queuesecret_value        = each.value["storage_account_queuesecret_value"]
    tags                     = var.rg_tags
    key_vault_name           = module.key_vault.name
    depends_on               = [module.key_vault]
  }

# - Calling KeyVault module to create an azure key vault

  module "key_vault" {
    source = "../modules/KeyVault"
    name                            = "kv-${var.project_code}-${var.locationshortprefix}-${var.Environment}-kv-${var.order}"  //kv-wppcs-weu-test-kv-01
    resource_group_name             = module.resource_group.name
    location                        = module.resource_group.location
    enabled_for_deployment          = var.enabled_for_deployment
    enabled_for_disk_encryption     = var.enabled_for_disk_encryption
    enabled_for_template_deployment = var.enabled_for_template_deployment
    tenant_id                       = data.azurerm_client_config.current.tenant_id
    object_id                       = data.azurerm_client_config.current.object_id
    sku_name                        = var.key_vault_sku_name
    tags                            = var.rg_tags
  }

  module "Key_vaultsecret" {
    source   = "../modules/KeyVault Secret"
    key_vault_name               = module.key_vault.name
    resource_group_name          = module.resource_group.name
    kv_name                      = var.kv_name
    rg_name                      = var.rg_name
    apim_name                    = module.api_management.name
    secret_name                  = var.secret_name
    secret_name1                 = var.secret_name1
    secret_name2                 = var.secret_name2
    secret_name3                 = var.secret_name3
    secret_name4                 = var.secret_name4
    secret_name5                 = var.secret_name5
    secret_name6                 = var.secret_name6
    secret_name7                 = var.secret_name7
    secret_name8                 = var.secret_name8
    secret_name9                 = var.secret_name9
    secret_name10                = var.secret_name10
    secret_name11                = var.secret_name11
    value1                       = var.value1
    value2                       = var.value2
    value3                       = var.value3
    value4                       = var.value4
    value5                       = var.value5
    value6                       = var.value6
    value7                       = var.value7
    value8                       = var.value8
    value9                       = var.value9
    value10                      = var.value10
    tags                         = var.rg_tags
    providers = {
        azurerm = azurerm
        azurerm.kv = azurerm.kv
    }
    depends_on                   = [module.key_vault]
  }

# - Calling FunctionApp module to create an Azure Function App
  module "function_app" {
    source = "../modules/FunctionApp"
    for_each                     = var.function_apps
    name                         = "func-${var.project_code}-${var.locationshortprefix}-${var.Environment}-${each.value["function_app_purpose"]}-${var.order}" //func-wppcs-weu-dev-prodcontroller-01
    resource_group_name          = module.resource_group.name
    location                     = module.resource_group.location
    appserviceplan_name          = "asp-${var.project_code}-${var.locationshortprefix}-${var.Environment}-${each.value["function_app_purpose"]}-${var.order}" //asp-wppcs-weu-dev-prodcontroller-01
    storage_account_name         = "sa${var.project_code}${var.locationshortprefix}${var.Environment}${each.value["function_app_sa_purpose"]}${var.tiershortprefix}${var.order}" 
    key_vault_name               = module.key_vault.name
    kv_name                      = each.value["function_app_kv_name"]
    rg_name                      = each.value["function_app_rg_name"]
    tier                         = each.value["function_app_tier"]       //"Dynamic","Standard"
    size                         = each.value["function_app_size"]
    app_scale_limit              = each.value["function_app_scale_limit"]
    kvsecret_enable              = each.value["function_app_kvsecret_enable"]
    secret_name                  = "kv-func-${each.value["function_app_purpose"]}-uri" //kv-func-datamovement-uri
    tags                         = var.rg_tags
    FUNCTIONS_WORKER_RUNTIME     = each.value["function_app_FUNCTIONS_WORKER_RUNTIME"]
    appinsights_name             = module.appinsights.name
    providers = {
        azurerm = azurerm
        azurerm.kv = azurerm.kv
    }
    depends_on                   = [module.appinsights, module.storage_account,module.key_vault]      
  }

# - Calling Appinsights module to create an Azure App Insights
  module "appinsights" {
    source = "../modules/Application Insights"
    name                         = "appi-${var.project_code}-${var.locationshortprefix}-${var.Environment}-appinsights-${var.order}" //appi-wppcs-weu-dev-appinsights-01
    resource_group_name          = module.resource_group.name
    location                     = module.resource_group.location
    tags                         = var.rg_tags
  }

# - Calling SQL Server Module to create an SQL server and SQLDB

  module "sqlserver" {
    source = "../modules/AzureSQL"
    name                         = "sql-${var.project_code}-${var.locationshortprefix}-${var.Environment}-sql-${var.order}" //var.sqlserver_name //sql-wppcs-weu-test-01
    sqldatabase_name             = var.sqldatabase_name
    administrator_login          = var.administrator_login
    administrator_login_password = var.administrator_login_password
    firewall_rulenames           = var.firewall_rulenames
    start_ip_address             = var.start_ip_address
    end_ip_address               = var.end_ip_address
    resource_group_name          = module.resource_group.name
    location                     = module.resource_group.location
    secret_name                  = "kv-connectionstring-${var.sql_server_secret_name}" //kv-connectionstring-sqldb
    key_vault_name               = module.key_vault.name
    tags                         = var.rg_tags
    depends_on                   = [module.key_vault]
    value                        = "Server=tcp:sql-${var.project_code}-${var.locationshortprefix}-${var.Environment}-sql-${var.order}.database.windows.net,1433;Initial Catalog=${var.sqldatabase_name};Persist Security Info=False;User ID=${var.administrator_login};Password=${var.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }

# - Calling App Service module to create an Azure App Service

  module "app_service" {
    source = "../modules/App Service"
    name                         = "${var.project_code}-${var.app_service_purpose}-${var.Environment}-${var.order}" //wppcs-cloudstudioportal-qa-01
    resource_group_name          = module.resource_group.name
    location                     = module.resource_group.location
    appserviceplan_name          = "asp-${var.project_code}-${var.locationshortprefix}-${var.Environment}-${var.app_service_purpose}-${var.order}" //asp-wppcs-weu-qa-prodcontroller-01
    tags                         = var.rg_tags
    tier                         = var.tier        //"Dynamic","Standard"
    size                         = var.size
    appinsights_name             = module.appinsights.name
    depends_on                   = [module.appinsights]      
  }

# - Calling API Management module to create an APIM

  module "api_management" {
    source = "../modules/API Management"
    name                         = "apim-${var.project_code}-${var.locationshortprefix}-${var.Environment}-apigateway-${var.order}" //apim-wppcs-neu-t-apigateway-01
    resource_group_name          = module.resource_group.name
    location                     = module.resource_group.location
    publisher_name               = var.publisher_name
    publisher_email              = var.publisher_email
    sku_name                     = var.sku_name            //"Basic_1"
    tags                         = var.rg_tags
    webappname                   = module.app_service.name
    appinsights_name             = module.appinsights.name
    depends_on                   = [module.appinsights,module.storage_account,module.key_vault,module.sqlserver,module.app_service,module.function_app,module.resource_group]      
  }

