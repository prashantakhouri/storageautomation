@api @owner=Sumanth.Gogineni @testplan=280 @testsuite=741 @parallel=false
Feature: ProductionDataStoreInSQLDatabase
As a user I need to verify the Production level meta data in Azure SQL database

@testcase=749 @bvt @priority=1 @sqldbvalidation @DeleteProduction @version=5
Scenario: Verify record is created in the database whenever production is created
	When I send the "Create Production" API with below data for "ProductionStore1" Productionstore
		| Key                       | Value                      |
		| tokens[0].productionToken | Test-Automation-Production |
	Then I receive valid HTTP response code "200"
	And Verify Production "Test-Automation-Production" is created in "ProductionStore1" production store in "5" seconds
	And I receive sync response for 'Create Production' API
	And the response should be perisisted in the sql database with id, last sync time, offline status

@testcase=750 @bvt @priority=1 @sqldbvalidation @version=2
Scenario: Verify last sync date is updated in the database whenever single production store is archived
	When I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
	Then I receive valid HTTP response code "200"
	And I verify if the API execution is "Completed" with in 30 sec
	And I verify that all folders and sub folders under "ProductionStore1" Production store are archived
	Then the last sync date and time should be perisisted in the database

@testcase=751 @priority=2 @sqldbvalidation @version=3
Scenario: Verify last sync date is updated in the database whenever all production stores are archived
	When I send the "Archive All Productionstores" POST request
	Then I receive valid HTTP response code "200"
	And I verify if the API execution is "Completed" with in 5 min
	And I verify that all Productions and its files under all Production stores are archived
	Then the last sync date and time should be perisisted in the database for all production stores

@testcase=752 @priority=3 @sqldbvalidation @version=2
Scenario: Verify record is not created in the database whenever invalid request send to create production endpoint
	When I send the "Create Production" API with "tokens[0].productionToken" with value "ProdWith *" for "ProductionStore1" Productionstore
	Then I receive valid HTTP response code "422"
	And record with particular production name should not be created in the database

@testcase=753 @priority=3 @sqldbvalidation @version=5
Scenario: Verify record is not updated in the database whenever invalid request send to single production store archived endpoint
	When I send the "Archive A Productionstore" POST request for "invalidproductionstore" Productionstore
	Then I receive valid HTTP response code "404"
	And last sync date and time of the record with particular production name should not be updated in the database

@testcase=754 @priority=3 @ignore @manual
Scenario: Verify record is not updated in the database whenever invalid request send to all production stores archived endpoint
	When I send an Invalid request to archived production endpoint
	Then request should be failed with "400"
	And last sync date and time of the record with particular production name should not be updated in the database