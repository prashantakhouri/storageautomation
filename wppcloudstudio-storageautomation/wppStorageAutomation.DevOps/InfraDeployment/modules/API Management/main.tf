# Create an API Management
data "azurerm_app_service" "this" {
  name                  = var.webappname
  resource_group_name   = var.resource_group_name
}

resource "azurerm_api_management" "this" {
  name                  = var.name
  location              = var.location
  resource_group_name   = var.resource_group_name
  publisher_name        = var.publisher_name
  publisher_email       = var.publisher_email
  tags                  = var.tags
  sku_name              = var.sku_name  //"Basic_1"  
  
  identity {
    type = "SystemAssigned"   
  }

  policy {
  xml_content = <<XML
<policies>
    <inbound>
        <cors allow-credentials="true">
            <allowed-origins>
                <origin>https://${var.name}.developer.azure-api.net</origin>
                <origin>https://${data.azurerm_app_service.this.name}.azurewebsites.net</origin>
            </allowed-origins>
            <allowed-methods preflight-result-max-age="300">
                <method>*</method>
            </allowed-methods>
            <allowed-headers>
                <header>*</header>
            </allowed-headers>
            <expose-headers>
                <header>*</header>
            </expose-headers>
        </cors>
    </inbound>
    <backend>
        <forward-request />
    </backend>
    <outbound />
    <on-error />
</policies>

XML
  }
  
}

data "azurerm_application_insights" "this" {
  name                  = var.appinsights_name
  resource_group_name   = var.resource_group_name
}
# -- Assign app insights to the APIM
resource "azurerm_api_management_logger" "this" {
  name                  = data.azurerm_application_insights.this.name
  api_management_name   = azurerm_api_management.this.name
  resource_group_name   = var.resource_group_name

  application_insights {
    instrumentation_key = data.azurerm_application_insights.this.instrumentation_key
  }
}

