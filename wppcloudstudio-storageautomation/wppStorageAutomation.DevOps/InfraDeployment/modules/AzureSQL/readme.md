### Copyright (c) Microsoft Corporation. All rights reserved.
### Customer - WPP Cloud Studio(WPP)
### Module Summary - This module generates azure sql server.

------------

###  Input variables - Inputs required by the module
-  #####  resource_group_name(resource group name)
-  #####  location(location for the storage account)
-  #####  tags(tags for the resource group)
-  #####  name(name of the Azure SQL server)
-  #####  administrator_login(Admin login of sql server)
-  #####  administrator_login_password(admin password for SQL Server)
-  #####  sqldatabase_name(SQL DB name)
-  #####  secret_name(key vault secret name for SQL DB)
-  #####  key_vault_name(key vault name)
-  #####  value(key vault secret value)
------------

###  Output variables - Outputs required by the module
-  #####  id(SQL server id)
-  #####  name(SQL server name)
-  #####  location(SQL server location)
-  #####  principal_id(principal_id of sql server)
-  #####  tenant_id(tenant_id of the SQL server)


