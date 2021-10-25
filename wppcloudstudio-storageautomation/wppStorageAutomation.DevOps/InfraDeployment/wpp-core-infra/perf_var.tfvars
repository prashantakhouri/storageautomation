# - Resource group variable values

   rg_tags = {
      project           = "wppcs" 
      environment       = "perf"
      #partner_id = "cbc371cc-5052-479b-b48e-51bb779545e4"
  }

   project_code         = "wppcs"
   tiershortprefix      = "stand" //prem for premium


  # -- Storage Account variable values
  storage_accounts = {
      storage_account1 ={
         storage_account_purpose               = "fnc"
         storage_account_tier                  = "Standard"
         storage_account_repl_type             = "LRS"
         storage_account_account_kind          = "StorageV2"
         storage_account_kvsecret_enable       = true
         storage_account_queue_enable          = false
         storage_account_queue_name            = null 
         storage_account_queuekvsecret_enable  = false
         storage_account_queuesecret_name      = null //kv-QueueStorageName  
         storage_account_queuesecret_value     = null                
      }

      storage_account2 ={
         storage_account_purpose               = "que"
         storage_account_tier                  = "Standard"
         storage_account_repl_type             = "LRS"
         storage_account_account_kind          = "StorageV2"
         storage_account_kvsecret_enable       = true
         storage_account_queue_enable          = true
         storage_account_queue_name            = "archive-queue"
         storage_account_queuekvsecret_enable  = true
         storage_account_queuesecret_name      = "kv-queuestoragename"  
         storage_account_queuesecret_value     = "archive-queue"         
      }
   }

# -- Function App variables values

   function_apps = {

      function_app1 ={
         function_app_purpose                  = "prodcontroller"
         function_app_FUNCTIONS_WORKER_RUNTIME = "dotnet"
         function_app_kvsecret_enable          = false
         function_app_tier                     = "Dynamic"
         function_app_size                     = "Y1"
         function_app_kv_name                  = "kv-wppcs-uks-t-kv-01"
         function_app_rg_name                  = "rg-wppcs-uks-t-core-hub-01"
         function_app_sa_purpose               = "fnc"
      }

      function_app2 ={
         function_app_purpose                  = "datamovement"
         function_app_FUNCTIONS_WORKER_RUNTIME = "dotnet"
         function_app_kvsecret_enable          = true
         function_app_tier                     = "Dynamic"
         function_app_size                     = "Y1"
         function_app_kv_name                  = "kv-wppcs-uks-t-kv-01"
         function_app_rg_name                  = "rg-wppcs-uks-t-core-hub-01"
         function_app_sa_purpose               = "fnc"
      }
      function_app3 ={
         function_app_purpose                  = "arcscheduler"
         function_app_FUNCTIONS_WORKER_RUNTIME = "dotnet"
         function_app_kvsecret_enable          = false
         function_app_tier                     = "Dynamic"
         function_app_size                     = "Y1"
         function_app_kv_name                  = "kv-wppcs-uks-t-kv-01"
         function_app_rg_name                  = "rg-wppcs-uks-t-core-hub-01"
         function_app_sa_purpose               = "fnc"
      }

      function_app4 ={
         function_app_purpose                  = "productionstore"
         function_app_FUNCTIONS_WORKER_RUNTIME = "dotnet"
         function_app_kvsecret_enable          = false
         function_app_tier                     = "Dynamic"
         function_app_size                     = "Y1"
         function_app_kv_name                  = "kv-wppcs-uks-t-kv-01"
         function_app_rg_name                  = "rg-wppcs-uks-t-core-hub-01"
         function_app_sa_purpose               = "fnc"
      }

      function_app5 ={
         function_app_purpose                  = "arcqueuestorage"
         function_app_FUNCTIONS_WORKER_RUNTIME = "dotnet"
         function_app_kvsecret_enable          = false
         function_app_tier                     = "Dynamic"
         function_app_size                     = "Y1"
         function_app_kv_name                  = "kv-wppcs-uks-t-kv-01"
         function_app_rg_name                  = "rg-wppcs-uks-t-core-hub-01"
         function_app_sa_purpose               = "que"
      }
   }

# -- Key Vault variables values
   
   key_vault_sku_name              = "standard"
   enabled_for_disk_encryption     = "true"
   enabled_for_template_deployment = "true"
   enabled_for_deployment          = "true"

# -- Key Vault secret name variable vaule   

   secret_name                     = "kv-storage-keyvault-uri" //thomas keyvault name in value
   kv_name                         = "kv-wppcs-uks-t-kv-01"
   rg_name                         = "rg-wppcs-uks-t-core-hub-01"
   secret_name1                    = "kv-authserver-app-clientid"
   secret_name2                    = "kv-authserver-app-url"
   secret_name3                    = "kv-func-datamovement-appreg-id"
   secret_name4                    = "kv-sddl-create-production"
   secret_name5                    = "kv-sddl-readonly-makeoffline"
   secret_name6                    = "kv-graphapi-clientid"
   secret_name7                    = "kv-graphapi-clientsecret"
   secret_name8                    = "kv-graphapi-domain"
   secret_name9                    = "kv-authserver-app-oauth-url" //if any new secret added need to add here
   secret_name10                   = "kv-sddl-fullcontrol-config"
   secret_name11                   = "kv-apim-base-url"

# -- SQLServer and SQLDB variables vaules
   
   sqldatabase_name                = "wppsqldb"
   sql_server_secret_name          = "sqldb"
   firewall_rulenames              = "AllAzureIP"
   start_ip_address                = "0.0.0.0"
   end_ip_address                  = "0.0.0.0"

# -- App Service Variables values

   app_service_purpose             = "cloudstudioportal"
   tier                            = "Premium"       //"Dynamic","Standard" for prod Premium P1V3
   size                            = "P1V3"

# -- APIM variables values

   publisher_name                  = "WPP Cloud Studio"
   publisher_email                 = "wppdevopsteam@microsoft.com"
   sku_name                        = "Standard_4"
