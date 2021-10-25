@backend @owner=aryamol.jacob @testplan=280 @testsuite=1928 @parallel=false
Feature: ACLOnProduction
Schedule the Arching to happen every 15 min

@testcase=2191 @ACLValidation @priority=1 @bvt @manual @version=2
Scenario: Verify if the Group Name and SID are populated in Group table while registering a production store
Given I am authenticated as "admin"
When I send the "Create a Production store" API with below data
| Key  | Value                       |
| Name | testcreateproductionstore |
Then I receive valid HTTP response code "200"
And I receive a sync response for 'Create a Production store' API
And production store data is registered in database
And I verify if the Manager Group Name, User Group Name and corresponding SIDs are populated in Group table

@testcase=2192 @ACLValidation @priority=1 @manual @notautomatable @version=2
Scenario: Verify when a Production is created, users belonging to valid group is able to access the Productions as per the defined ACL
When  I send the "Create Production" API with below data for "ProductionStore1" Productionstore
| Key                       | Value             |
| tokens[0].productionToken | Test-Production_1 |
Then I receive valid HTTP response code "200"
And Verify Production "Test-Production_1" is created in "ProductionStore1" production store in "5" seconds
And I receive sync response for 'Create Production' API
And Verify "success" sync response body for "Create Production" API
| Key  | Value             |
| name | Test-Production_1 |
| uri  | Test-Production_1 |
And I verify if "Manager Group1 ProductionStore1" users are able to access "Test-Production_1" from VM
And I Verify that "Manager Group1 ProductionStore1" users have read only access in Productions level
And I Verify that "Manager Group1 ProductionStore1" users have full access to all subfolders of "Test-Production_1"
And I verify if "User Group1 ProductionStore1" users are able to access "Test-Production_1" from VM
And I Verify that "User Group1 ProductionStore1" users have read only access in Productions level
And I Verify that "User Group1 ProductionStore1" users have full access to all subfolders of "Test-Production_1"
And I verify if "Manager Group2 ProductionStore2" users are not able to access "Test-Production_1" from VM
And I verify if "User Group2 ProductionStore2" users are not able to access "Test-Production_1" from VM

@testcase=2193 @ACLValidation @priority=1 @bvt @manual @notautomatable @version=2
Scenario: Verify when a Production is Restored, users belonging to valid group is able to access the Productions as per the defined ACL
Given I send the "Archive A Productionstore" POST request for "ProductionStore3" Productionstore
And I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
And I verify that all folders and sub folders under "ProductionStore3" Production store are archived
And I delete "ProductionStore3_Production1" Production from "ProductionStore3" Production Store
When I send "Restore Production" API to restore "ProductionStore3_Production1" Production for "ProductionStore3" production store
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
And Verify Production "ProductionStore3_Production1" is restored in "ProductionStore3" production store in "5" seconds
And I verify that all folders and sub folders under "ProductionStore3_Production1" are restored
And I verify if "Manager Group1 ProductionStore3" users are able to access "Test-Production_1" from VM
And I Verify that "Manager Group1 ProductionStore3" users have read only access in Productions level
And I Verify that "Manager Group1 ProductionStore3" users have full access to all subfolders of "Test-Production_1"
And I verify if "User Group1 ProductionStore3" users are able to access "Test-Production_1" from VM
And I Verify that "User Group1 ProductionStore3" users have read only access in Productions level
And I Verify that "User Group1 ProductionStore3" users have full access to all subfolders of "Test-Production_1"
And I verify if "Manager Group2 ProductionStore2" users are not able to access "Test-Production_1" from VM
And I verify if "User Group2 ProductionStore2" users are not able to access "Test-Production_1" from VM

@testcase=4055 @ACLValidation @priority=1 @bvt @manual @notautomatable @testplan=280 @testsuite=4054
Scenario: Verify when production is restoring from wpp portal then production should be in read only till restore completed
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store
And I have logged into VDI
And I have mounted "ProductionStore1"
When I have "Production1_ProductionStore1" is in offline state
And I should not be able to see "Production1_ProductionStore1" in "ProductionStore1" drive of VDI
And I have cliked on make online for "Production1_ProductionStore1" from portal
Then I should see "Production1_ProductionStore1" folder in VDI
And "Production1_ProductionStore1" should be in readonly till restore completes
When restore for "Production1_ProductionStore1" is completed
Then "Production1_ProductionStore1" should be set with special permissions