# -- Variables for Application Insights

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

variable "tags" {
  type = map(string)
  description = "Tag details for the resouce" 
}
