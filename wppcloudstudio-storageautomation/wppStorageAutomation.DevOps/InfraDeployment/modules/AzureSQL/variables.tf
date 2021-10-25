# -
# - Variables created for the SQL Server and SQLDB 
# -

variable "location" {
  description = "Resource Group Location"
}

variable "name" {
  description = "SQL Server name"
}

variable "sqldatabase_name" {
  description = "SQL DB name"
}

variable "resource_group_name" {
  description = "RG name"
}

variable "administrator_login" {
  description = "SQL Admin Name"
}

variable "administrator_login_password" {
  description = "SQL Admin password"
}

variable "tags" {
  type = map(string)
  description = "Environment tag for the resource group (i.e. 'Production')"
}

variable "secret_name" {
   type = string
   default = null
}
variable "key_vault_name" {
   type = string
}

variable "value"{
  type = string
}
variable "firewall_rulenames" {
  type = string
}

variable "start_ip_address" {
  type = string
}

variable "end_ip_address" {
  type = string
}


