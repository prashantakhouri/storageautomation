# -
# - Variables created for the Stoarge Account 
# -

variable "resource_group_name" {
    description = "resource group name"
    type = string
}

variable "location" {
    description = "resource group location"
    type = string
}

variable "name" {
    description = "storage account name"
    type = string
}

# variable "container_name" {
#     description = "storage account container name"
#     type = string
#     default = null
# }

# variable "container_access_type" {
#     description = "storage account container access type"
#     type = string
#     default = null
# }
# variable "fileshare_name" {
#     description = "storage account fileshare name"
#     type = string
#     default = null
# }
# variable "quota" {
#     description = "storage account fileshare quota"
#     type = number
#     default = null
# }
variable "account_tier" {
    description = "storage account tier"
    type = string
}
 variable "account_kind" {
   description = "kind of storage account -blob, etc"
   type = string
 }
variable "account_replication_type" {
    description = "storage account replication type"
    type = string
}
variable "tags" {
  type = map(string)
  description = "Tag details for the resouce"
}
# variable "fileshare_enable" {
#    type = bool
#    description = "To enable file share"
# }
# variable "container_enable" {
#    type = bool
#    description = "To enable container"
# }
variable "kvsecret_enable" {
   type = bool
   description = "To enable KV secret"
}
variable "secret_name" {
   type = string
   default = null
   description = "Name of the secret in Keyvault"
}
variable "key_vault_name" {
   type = string
   description = "keyvault name"
}
variable "queue_enable" {
   type = bool
   description = "To enable queue"
}

variable "queue_name" {
    description = "storage account queue name"
    type = string
    default = null
}
variable "queuekvsecret_enable" {
   type = bool
   description = "To enable queue secret"
}

variable "queuesecret_name" {
    description = "storage account queue secret name"
    type = string
    default = null
}

variable "queuesecret_value" {
    description = "storage account queue secret value"
    type = string
    default = null
}

