### Copyright (c) Microsoft Corporation. All rights reserved.
### Customer - WPP Cloud Studio(WPP)
### Module Summary - This module generates azure key vault.

------------


###  Input variables - Inputs required by the module
-  #####  name(name of the key vault)
-  #####  resource_group_name(resource group for the key vault)
-  #####  location(location for the key vault)
-  #####  sku_name(key vault sku name - Standard/Premium)
-  #####  tenant_id(tenant id of the subscription for key vault)
-  #####  object_id(object id of the subscription for key vault)
-  #####  tags(tags for the key vault)
-  #####  enabled_for_disk_encryption(enable disk encryption for the key vault)
-  #####  enabled_for_template_deployment(enable for template deployment for the key vault)
-  #####  enabled_for_deployment(enable for template deployment)
-  #####  key_vault_id(key vault id of the key vault)
-  #####  object_id(object_id of the user)

------------

###  Output variables - Outputs required by the module
-  #####  id(key vault id)
-  #####  name(key vault name)
-  #####  location(key vault location)
