# Create an azure app service plan

resource "azurerm_app_service_plan" "this" {
  name                                       = var.appserviceplan_name
  location                                   = var.location
  resource_group_name                        = var.resource_group_name
  tags                                       = var.tags
  sku {
    tier =  var.tier        //"Dynamic","Standard"
    size =  var.size              //"Y1","S1"
  }
}

data "azurerm_application_insights" "this" {
  name                                        = var.appinsights_name
  resource_group_name                         = var.resource_group_name
}

# - Function App for App Service Plan

resource "azurerm_app_service" "this" {
  name                                        = var.name
  location                                    = var.location
  resource_group_name                         = var.resource_group_name
  app_service_plan_id                         = azurerm_app_service_plan.this.id
  tags                                        = var.tags
  https_only                                  = "true"
  identity {
    type = "SystemAssigned"   
  }
  app_settings = {
        APPINSIGHTS_INSTRUMENTATIONKEY        = data.azurerm_application_insights.this.instrumentation_key 
        APPLICATIONINSIGHTS_CONNECTION_STRING = data.azurerm_application_insights.this.connection_string
    }

}