@backend @owner=Sumanth.Gogineni @testplan=280 @testsuite=1929 @parallel=false
Feature: HandlingMultipleStorageAccounts
As a production manager I want production stores
in different regions so that performance in optimal
for the studio using the storage

@testcase=1977 @bvt @priority=1 @version=6 @DeleteParticularProductionStore
Scenario: Verify Connection string key name should stored in the database
	Given I am authenticated as "admin"
	And that I provide the connection string key vault key name in the register production store endpoint request body
	When I send the "Create a Production store" API with below data
		| Key  | Value                     |
		| Name | testcreateproductionstore |
	Then I receive valid HTTP response code "200"
	And production store data is registered in database
	And the value of "WIPKeyName" persists in the database
	And the value of "ArchiveKeyName" persists in the database

@testcase=1978 @priority=2 @version=3 @DeleteProduction
Scenario: Verify Production is created in particular production store
	Given I am authenticated as "admin"
	And that I have access to a particular production store
	When  I send the "Create Production" API with below data for "ProductionStore1" Productionstore
		| Key                       | Value             |
		| tokens[0].productionToken | Test-Production_1 |
	Then I receive valid HTTP response code "200"
	And Verify Production "Test-Production_1" is created in "ProductionStore1" production store in "5" seconds

@testcase=1979 @priority=2 @version=3 @DeleteMultipleProductions
Scenario: Verify Archived produtions should store in correct archive production stores
	Given I am authenticated as "admin"
	And that there are multiple production stores in different storage accounts
	And created productions in "ProductionStore4" and "ProductionStore5" which are in different storage accounts
	When I send the "Archive All Productionstores" POST request
	Then I receive valid HTTP response code "200"
	And I verify if the API execution is "Completed" with in 4 min
	And the archive is created in the correct production store archive storage account