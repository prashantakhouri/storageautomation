# -
# - Variables created for the module : KeyVault
# -

variable "key_vault_name" {
    type = string
    description ="Name of the Key Vault"
}
variable "resource_group_name" {
    type = string
    description ="Name of the Resource Group"
}
variable "tags" {
  type = map(string)
  description = "Tag details for the resouce"
}
variable "kv_name" {
  type = string
}
variable "rg_name" {
  type = string
}
variable "apim_name" {
  type = string
}
variable "secret_name" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}

variable "secret_name1" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}

variable "secret_name2" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}

variable "secret_name3" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}
variable "secret_name4" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}
variable "secret_name5" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}
variable "secret_name6" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}
variable "secret_name7" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}
variable "secret_name8" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}
variable "secret_name9" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}
variable "secret_name10" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}

variable "secret_name11" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}

variable "value1" {
   type = string
}

variable "value2" {
   type = string
}

variable "value3" {
   type = string
}
variable "value4" {
   type = string
}
variable "value5" {
   type = string
}
variable "value6" {
   type = string
}
variable "value7" {
   type = string
}
variable "value8" {
   type = string
}

variable "value9" {
   type = string
}
variable "value10" {
   type = string
}


