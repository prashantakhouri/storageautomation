### Copyright (c) Microsoft Corporation. All rights reserved.
### Customer - WPP Cloud Studio(WPP)
### Module Summary - This module generates azure storage account.

------------

###  Input variables - Inputs required by the module
-  #####  resource_group(resource group for the storage account)
-  #####  location(location for the storage account)
-  #####  account_tier(storage account tier - Standard/Premium)
-  #####  account_tier(storage account tier - Standard/Premium)
-  #####  account_replication_type(replication type of storage account - LRS/GRS)
-  #####  account_kind(account_kind of storage account)
-  #####  name(name of the storage account)
-  #####  storage_account_name(storage account name where container will be created)
-  #####  container_access_type(container access type for the container)
-  #####  container_name(containwe name created inside the storage account)
-  #####  fileshare_name(file share name created inside the storage account)
-  #####  secret_name(name of the key vault secret)
-  #####  quota(the size of the storage account)

-  #####  tags(tags for the storage account)
------------

###  Output variables - Outputs required by the module
-  #####  sas_token(primary access token of the storage account)
-  #####  id(storage account id)
-  #####  name(storage account name)
-  #####  location(storage account location)
-  #####  connection_string_primary(storage account connection string)