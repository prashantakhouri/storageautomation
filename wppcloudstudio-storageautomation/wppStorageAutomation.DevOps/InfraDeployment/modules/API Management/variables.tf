# -- Variables for APIM
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
}
variable "publisher_name" {
  type = string
}
variable "publisher_email" {
  type = string
}
variable "sku_name" {
  type = string
}
variable "tags" {
  type = map(string)  
}
variable "appinsights_name" {
  type = string
}
variable "webappname" {
  type = string
}
