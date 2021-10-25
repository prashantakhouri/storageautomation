# --
# -- This file holds all the variable details for tanla core infra master pipleine
# --

# -- Environment variables details
  variable "Environment" {
    description = "Environment name"
  }
  variable "locationshortprefix" {
    description = "location short prifix"
  }
  variable "tiershortprefix" {
    description = "tier short prifix"
  }
  variable "order" {
    description = "order"
  }
  variable "hub_sub" {
    type = string
    description = "subscription ID"
  }
  variable "main_sub" {
    type = string
    description = "subscription ID"
  }

  variable "project_code" {
    type = string
  }
# -- Resouce Group variable details

  variable "rg_location" {
    description = "Resource Group Location"
  }

  #variable "rg_name" {
  #  description = "Resource Group Name"
  #}

  variable "rg_tags" {
    type = map(string)
    description = "Environment tag for the resource group (i.e. 'Production')"
  }

# -- Storage account variable details

  variable "storage_accounts" {
    type = map(object({
      storage_account_purpose               = string
      storage_account_tier                  = string
      storage_account_repl_type             = string
      storage_account_account_kind          = string
      storage_account_kvsecret_enable       = bool
    }))
    description = "Specifies the map of attributes for storage_accounts."
    default     = {}
  }
#storage_account_Keyvaultsecret_name   = strin
# -- Function App variable details
  variable "function_apps" {
    type = map(object({
      function_app_purpose                  = string
      function_app_FUNCTIONS_WORKER_RUNTIME = string
      function_app_kvsecret_enable          = bool
      function_app_kv_name                  = string
      function_app_rg_name                  = string
    }))
      description = "Specifies the map of attributes for function_apps."
      default   = {}
  }

# -- App Insights Variable details

#variable "application_insights_purpose" {
#  description = "Application Insights deployment purpose"
#  type        = string
#}

  # -- KeyVault variable details
  variable "key_vault_sku_name" {
    type= string
    description = "key vault sku"
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
  variable "kv_name" {
  type = string
  }
  variable "rg_name" {
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
# -- SQL Server variables details

  variable "sql_server_secret_name" {
    type= string
    description = "secret name for the SQL server in Keyvault"
  }
  
 variable "sqldatabase_name" {
    type= string 
    description = "SQL DB name"
  }

  # variable "sqlserver_name" {
  #   type= string 
  #   description = "SQL Server name"   
  # }

variable "administrator_login" {
  type= string
  description = "SQL Admin Name"
  }

variable "administrator_login_password" {
  type= string
  description = "SQL Admin password"
}

variable "firewall_rulenames" {
  type        = string
}

variable "start_ip_address" {
  type        = string
}

variable "end_ip_address" {
  type        = string
}

# -- Azure App Service variables details

variable "app_service_purpose" {
  description = "Application Insights deployment purpose"
  type        = string
}
variable "tier" {
  type = string
  description = "App Service tier"
}
variable "size" {
  type = string
  description = "App Service size"
}

# -- Azure APIM variables details

variable "publisher_name"{
  type = string
}

variable "publisher_email"{
  type = string
}

variable "sku_name"{
  type = string
}






  



