#Set the terraform required version
terraform {
  required_version = "~> 1.0.0"
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "~> 2.65.0"
    }
  }
}

# Configure the default Azure Provider
provider "azurerm" {
  # It is recommended to pin to a given version of the Provider
  partner_id      = "cbc371cc-5052-479b-b48e-51bb779545e4"
  subscription_id = var.main_sub
  features {}
}
# Additional provider configuration for kv
provider "azurerm" {
  # It is recommended to pin to a given version of the Provider
  alias           = "kv"
  partner_id      = "cbc371cc-5052-479b-b48e-51bb779545e4"
  subscription_id = var.hub_sub
  features {}
}

# Data

# Make client_id, tenant_id, subscription_id and object_id variables
data "azurerm_client_config" "current" {}
