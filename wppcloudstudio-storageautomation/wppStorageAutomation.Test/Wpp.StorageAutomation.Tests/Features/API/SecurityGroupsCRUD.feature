@api @owner=saloni.shrivastava @testplan=280 @testsuite=2403 @parallel=false
Feature: SecurityGroupsCRUD
User can perform Production and Production Store API CRUD operations on Production Stores which user is member of the users or managers group for that Production Store.

@testcase=2525 @priority=1 @bvt @version=4
Scenario Outline:02Verify authorised user for a production store can execute all APIs for the production store
Given I add "<group name>" to "<group category>" security group for "ProductionStore6" production store
When I send the "Create Production" API with below data for "ProductionStore6" Productionstore
| Key                       | Value               |
| tokens[0].productionToken | Test-Production_Sec |
Then I receive valid HTTP response code "200"
And Verify Production "Test-Production_Sec" is created in "ProductionStore6" production store in "5" seconds
When I send the "List Productions" GET request for "ProductionStore6" Productionstore
Then I verify if all productions present in the WIP storage for "ProductionStore6" Productionstore are listed
When I send the "CreateAProductionStore" GET request
Then I receive valid HTTP response code "200"
When I get the list of files and folders before making "ProductionStore6_Production1" production offline
And I send "Make Production Offline" API to restore "ProductionStore6_Production1" Production for "ProductionStore6" production store
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
And I send "Restore Production" API to restore "ProductionStore6_Production1" Production for "ProductionStore6" production store
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
And Verify Production "ProductionStore6_Production1" is restored in "ProductionStore6" production store in "5" seconds
And I verify that all folders and sub folders under "ProductionStore6_Production1" are restored
Examples:
| group name                                        | group category         |
| AuthorizedUserGroup1                              | UserGroup              |
| AuthorizedManagerGroup1                           | ManagerGroup           |
| AuthorizedUserGroup1                              | ManagerGroup,UserGroup |
| AuthorizedUserGroup1,UnauthorizedUserGroup1       | UserGroup              |
| AuthorizedManagerGroup1,UnauthorizedManagerGroup1 | ManagerGroup           |

@testcase=2526 @priority=1 @bvt @version=4
Scenario: 01Verify unauthorised user for a production store cannot perform CRUD operations from API
Given I remove all groups for both security group for "ProductionStore6" production store
When  I send the "Create Production" API with below data for "ProductionStore6" Productionstore
| Key                       | Value |
| tokens[0].productionToken | Test8 |
Then I receive valid HTTP response code "403"
When I send the "List Productions" GET request for "ProductionStore6" Productionstore
Then I receive valid HTTP response code "403"
When I send the "CreateAProductionStore" GET request
Then Verify "ProductionStore6" is not present in the list of stores returned
When I send "Make Production Offline" API to restore "ProductionStore6_Production1" Production for "ProductionStore6" production store
Then I receive valid HTTP response code "403"
And I send "Restore Production" API to restore "ProductionStore6_Production1" Production for "ProductionStore6" production store
Then I receive valid HTTP response code "403"
Given I add "UnauthorizedUserGroup1" to "UserGroup" security group for "ProductionStore6" production store
And I add "UnauthorizedManagerGroup1" to "Managergroup" security group for "ProductionStore6" production store
When  I send the "Create Production" API with below data for "ProductionStore6" Productionstore
| Key                       | Value |
| tokens[0].productionToken | Test8 |
Then I receive valid HTTP response code "403"
When I send the "List Productions" GET request for "ProductionStore6" Productionstore
Then I receive valid HTTP response code "403"
When I send the "CreateAProductionStore" GET request
Then Verify "ProductionStore6" is not present in the list of stores returned
When I send "Make Production Offline" API to restore "ProductionStore6_Production1" Production for "ProductionStore6" production store
Then I receive valid HTTP response code "403"
And I send "Restore Production" API to restore "ProductionStore6_Production1" Production for "ProductionStore6" production store
Then I receive valid HTTP response code "403"
