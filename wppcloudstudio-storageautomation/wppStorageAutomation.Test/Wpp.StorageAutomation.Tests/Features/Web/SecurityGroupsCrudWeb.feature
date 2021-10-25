@ui @owner=saloni.shrivastava @testplan=280 @testsuite=2403 @parallel=false
Feature: SecurityGroupsCrudWeb
User can perform Production and Production Store Web CRUD operations on Production Stores which user is member of the users or managers group for that Production Store.

@testcase=2527 @priority=1 @bvt @DeleteProduction @version=3
Scenario Outline: 02Verify authorised user for a production store can perform CRUD operations from UI
Given I add "<group name>" to "<group category>" security group for "ProductionStore6" production store
When I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore6" production store in "Region1" region
Then Verify all productions are listed for "ProductionStore6" production store in "asc" order of "name"
When I click on "Create Production" button
Then I am redirected to "Create Production" page
And I enter "Test-Prod_SecGroup" into the "Production Name" text field
And I click on "Save" button
And Verify Production "Test-Prod_SecGroup" is created in "ProductionStore6" production store in "10" seconds
And Verify Production "Test-Prod_SecGroup" is created in "ProductionStore6" Production Store on UI
Examples:
| group name                                        | group category         |
| AuthorizedUserGroup1                              | UserGroup              |
| AuthorizedManagerGroup1                           | ManagerGroup           |
| AuthorizedUserGroup1                              | ManagerGroup,UserGroup |
| AuthorizedUserGroup1,UnauthorizedUserGroup1       | UserGroup              |
| AuthorizedManagerGroup1,UnauthorizedManagerGroup1 | ManagerGroup           |


@testcase=2528 @priority=1 @bvt @version=3
Scenario: 01Verify unauthorised user for a production store cannot perform CRUD operations
Given I remove all groups for both security group for "ProductionStore6" production store
When I have logged into Storage Automation portal using "admin" user
Then Verify "ProductionStore6" production store is not visible
When I naviagte to a an unauthorized production store "ProductionStore6"
Then I get a "You are not authorized to view <ProductionStoreName> Production store." banner mesaage
Given I add "UnauthorizedUserGroup1" to "UserGroup" security group for "ProductionStore6" production store
And I add "UnauthorizedManagerGroup1" to "Managergroup" security group for "ProductionStore6" production store
When I naviagte to a an unauthorized production store "ProductionStore6"
Then I get a "You are not authorized to view <ProductionStoreName> Production store." banner mesaage