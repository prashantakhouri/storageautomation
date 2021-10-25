@api @owner=saloni.shrivastava @testplan=280 @testsuite=740 @parallel=false
Feature: ListProductions
List all Productions under a production store

# ================= Test cases related to List Productions in a Production Store =============================#
@testcase=743 @listproduction @priority=1 @bvt @version=5
Scenario: Verify if details all Productions under a production store are available in List API Response
	When I send the "List Productions" GET request for "ProductionStore1" Productionstore
	Then I verify if all productions present in the WIP storage for "ProductionStore1" Productionstore are listed
	And I verify the details of each "production"

@testcase=744 @listproduction @priority=1 @bvt @version=5
Scenario: Verify if 404 error message occurs when an invalid production name is passed to List API
	When I send the "List Productions" GET request for "WrongProductionStoreName" Productionstore
	Then I receive valid HTTP response code "404"