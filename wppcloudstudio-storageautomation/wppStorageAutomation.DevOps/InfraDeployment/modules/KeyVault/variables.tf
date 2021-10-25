# -
# - Variables created for the module : KeyVault
# -

variable "name" {
    type = string
    description ="Name of the Key Vault"
}
variable "resource_group_name" {
    type = string
    description ="Name of the Resource Group"
}
variable "location" {
    type = string
    description ="Location of the Resource Group"
}
variable "sku_name" {
    type = string
    description ="SKU of the Key Vault" 
}
variable "tags" {
  type = map(string)
  description = "Tag details for the resouce"
}
variable "enabled_for_disk_encryption" {
    type = bool
    description ="KV enabled for Disk Encryption" 
}
variable "enabled_for_template_deployment" {
    type = bool
    description ="Key Vault enabled for Tempelate Deployment" 
}
variable "enabled_for_deployment" {
    type = bool
    description ="Key Vault enabled for Deployment" 
}
