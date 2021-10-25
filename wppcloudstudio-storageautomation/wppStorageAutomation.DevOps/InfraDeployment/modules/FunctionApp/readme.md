

### Copyright (c) Microsoft Corporation. All rights reserved.


### Customer - WPP Cloud Studio(WPP)


### Module Summary - This module generates azure Function App and setup of related config. 

----

### Input variables - Inputs required by the module

-  ##### resource_group(resource group for the Function App)
-  ##### location(location for the Function App)
-  ##### kind(Kind of Azure App Service Plan)
-  ##### name(name of the Function App
-  ##### sku(tier and size for the app service plan)
-  ##### storage_account_access_key(storage account access key for sku of the Function App)
-  ##### storage_account_name(storage account name for sku of the Function App)
-  ##### app_service_plan_id(app service plan for the Function App)
-  ##### FUNCTIONS_WORKER_RUNTIME(functions worker runtime for the Function App)
-  ##### WEBSITE_NODE_DEFAULT_VERSION(website node default version for Function App)
-  ##### AppInsights_InstrumentationKey(Instrumentation key of app insights for function app)
-  ##### type(identity type for the Function App)
-  ##### version(version of the Function App)
-  ##### tags(tags for the application gateway)
-------
### Output variables - Ouputs sent out from the module
-  ##### appserviceplan_id(application gateway id)
-  ##### name(function app name)
-  ##### principal_id(principal id of the function app)
-  ##### tenant_id(tenant id of the function app)
-  ##### location(location of the function app)