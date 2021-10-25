@api @owner=saloni.shrivastava @testplan=280 @testsuite=1288 @parallel=false
Feature: ListProductionStores
List all Production Stores in a region/pod

# ================= Test cases related to List Production Store in a region/pod =============================#

@testcase=1434 @bvt @priority=1 @version=1
Scenario: Verify if details of all Production Stores in API Response
When I send the "CreateAProductionStore" GET request
Then I verify if all production stores present are listed
And I verify the details of each "production store"

