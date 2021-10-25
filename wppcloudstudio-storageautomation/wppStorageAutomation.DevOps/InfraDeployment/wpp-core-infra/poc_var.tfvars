# - Resource group variable values

   rg_tags = {
      project           = "wppcs" 
      environment       = "poc"
      #partner_id = "cbc371cc-5052-479b-b48e-51bb779545e4"
  }

   project_code  = "wppcs"

   #Environment          = "d"
   #locationshortprefix  = "weu"
   #tiershortprefix      = "stand"
   #rg_location          = "West Europe"
   #order                = "01"
   application_insights_purpose = "appinsights"


  # -- Storage Account variable values
  storage_accounts = {
      storage_account1 ={
         storage_account_purpose               = "fncstand"
         storage_account_tier                  = "Standard"
         storage_account_repl_type             = "LRS"
         storage_account_account_kind          = "StorageV2"
         storage_account_kvsecret_enable       = true                
      }
   }

# -- Function App variables values

   function_apps = {

      function_app1 ={
         function_app_purpose                  = "prodcontroller"
         function_app_FUNCTIONS_WORKER_RUNTIME = "dotnet"
         function_app_kvsecret_enable          = false
         function_app_kv_name                  = "kv-wppcs-uks-d-kv-01"
         function_app_rg_name                  = "rg-wppcs-uks-d-core-hub-01"
      }

      function_app2 ={
         function_app_purpose                  = "datamovement"
         function_app_FUNCTIONS_WORKER_RUNTIME = "dotnet"
         function_app_kvsecret_enable          = true
         function_app_kv_name                  = "kv-wppcs-uks-d-kv-01"
         function_app_rg_name                  = "rg-wppcs-uks-d-core-hub-01"
      }
      function_app3 ={
         function_app_purpose                  = "arcscheduler"
         function_app_FUNCTIONS_WORKER_RUNTIME = "dotnet"
         function_app_kvsecret_enable          = false
         function_app_kv_name                  = "kv-wppcs-uks-d-kv-01"
         function_app_rg_name                  = "rg-wppcs-uks-d-core-hub-01"
      }

      function_app4 ={
         function_app_purpose                  = "productionstore"
         function_app_FUNCTIONS_WORKER_RUNTIME = "dotnet"
         function_app_kvsecret_enable          = false
         function_app_kv_name                  = "kv-wppcs-uks-d-kv-01"
         function_app_rg_name                  = "rg-wppcs-uks-d-core-hub-01"
      }
   }

# -- Key Vault variables values
   
   key_vault_sku_name              = "standard"
   enabled_for_disk_encryption     = "true"
   enabled_for_template_deployment = "true"
   enabled_for_deployment          = "true"

# -- Key Vault secret name variable vaule   

   secret_name                     = "kv-storage-keyvault-uri" //thomas keyvault name in value
   kv_name                         = "kv-wppcs-uks-d-kv-01"
   rg_name                         = "rg-wppcs-uks-d-core-hub-01"
   secret_name1                    = "kv-authserver-app-clientid"
   secret_name2                    = "kv-authserver-app-url"
   secret_name3                    = "kv-func-datamovement-appreg-id"
   secret_name4                    = "kv-sddl-create-production"
   #value1                          = "0oax6g79tdjq65wjh416" -var 'value1="0oax6g79tdjq65wjh416"'
   #value2                          = "https://devh-wpp.okta.com"
   #value3                          = "ef59328c-4f24-4908-9bbc-49d98dc6f525"
   #value4                          = $(sddl-create-production)"

# -- SQLServer and SQLDB variables vaules
   #sqlserver_name                  = "sql-wppcs-neu-d-02"
   sqldatabase_name                = "wppsqldb"
   sql_server_secret_name          = "sqldb"
   firewall_rulenames              = "AllAzureIP"
   start_ip_address                = "0.0.0.0"
   end_ip_address                  = "0.0.0.0"
   #value                           = "Server=tcp:sqlserver-wppcs-weu-dev-01.database.windows.net,1433;Initial Catalog=wppsqldb;Persist Security Info=False;User ID=wppsqladmin;Password=Microsoft~123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# -- App Service Variables values

   app_service_purpose  = "cloudstudioportal"

# -- APIM variables values

   publisher_name = "WPP Cloud Studio"
   publisher_email = "wppdevopsteam@microsoft.com"
   sku_name = "Basic_1"



 
 