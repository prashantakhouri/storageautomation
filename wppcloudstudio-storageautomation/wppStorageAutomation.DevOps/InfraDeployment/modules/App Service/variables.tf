# -- Variables for App Service
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
  description = "Name of the App Service"
}
variable "appinsights_name" {
  type = string
  description = "App Insight Name"
}
variable "appserviceplan_name" {
  type = string
  description = "App Service Plan Name"
}
variable "tier" {
  type = string
  description = "App Service tier"
}
variable "size" {
  type = string
  description = "App Service size"
}
variable "tags" {
  type = map(string)
  description = "Tag details for the resouce"  
}

