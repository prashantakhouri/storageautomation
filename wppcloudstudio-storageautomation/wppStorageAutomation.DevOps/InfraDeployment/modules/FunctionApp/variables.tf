# -- Variables for Function App
variable "resource_group_name" {
  description = "resource group name"
  type = string
}
variable "location" {
  description = "resource group location"
  type = string
}
variable "name" {
  type = string
  description = "Function App Name"
}
variable "appinsights_name" {
  type = string
  description = "App Insights Name"
}
variable "appserviceplan_name" {
  type = string
  description = "App Service plan Name"
}
variable "storage_account_name" {
  type = string
  description = "Storage Account Name"
}
variable "FUNCTIONS_WORKER_RUNTIME" {
  type = string
  description = "Function worker runtime details"
}
variable "tags" {
  type = map(string)
  description = "Tag details for the resouce"  
}
variable "key_vault_name" {
   type = string
   description = "Key Vault Name"
}
variable "kv_name" {
  type = string
}
variable "rg_name" {
  type = string
}
variable "kvsecret_enable" {
   type = bool
   description = "To enable KV secret"
}
variable "secret_name" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}
variable "tier" {
  type = string
  description = "App Service tier"
}
variable "size" {
  type = string
  description = "App Service size"
}
variable "app_scale_limit" {
  type = string
  description = "app Scale limit of App Service Plan"
}
