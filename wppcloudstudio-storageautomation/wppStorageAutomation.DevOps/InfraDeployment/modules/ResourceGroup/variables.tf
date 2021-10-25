# -
# - Variables created for the Resource Group 
# -

variable "location" {
  description = "Resource Group Location"
}

variable "name" {
  description = "Resource Group Name"
}

variable "tags" {
  type = map(string)
  description = "Tag details for the resouce"
}


