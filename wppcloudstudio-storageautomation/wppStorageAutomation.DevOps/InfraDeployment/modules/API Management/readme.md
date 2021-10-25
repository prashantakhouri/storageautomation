### Copyright (c) Microsoft Corporation. All rights reserved.
### Customer - WPP Cloud Studio(WPP)
### Module Summary - This module generates Azure App Service and setup of related config. 
### Input variables - Inputs required by the module

-  ##### resource_group(resource group for the APIM)
-  ##### location(location for the APIM)
-  ##### publisher_name(Publisher name for the APIM)
-  ##### name(name of the APIM)
-  ##### api_management_name(APIM name)
-  ##### webappname(name of the web application)
-  ##### instrumentation_key(instrumentation_key of the APP insights)
-  ##### sku_name(sku details for APIM)
-  ##### appinsights_name(app insights name for APIM)
-  ##### publisher_email(publisher email ID for APIM)
-  ##### APPINSIGHTS_INSTRUMENTATIONKEY(App Insights instrumentation key)
-  ##### APPLICATIONINSIGHTS_CONNECTION_STRING(App Insights Connection String)
-  ##### tags(tags for the APIM)
-------
### Output variables - Ouputs sent out from the module
-  ##### apim_id(APIM ID)
-  ##### instrumentation_key(App Insights instrumentation Key)
-  ##### name(name of the APIM)
-  ##### location(location of the APIM)
