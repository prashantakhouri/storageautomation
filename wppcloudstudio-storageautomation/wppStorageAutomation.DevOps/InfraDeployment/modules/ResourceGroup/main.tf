# -
# - Create Resource Groups and assign mandatory tags
# - 

resource "azurerm_resource_group" "this" {
  name     = var.name
  location = var.location
  tags     = var.tags
}
