@ui @owner=saloni.shrivastava @testplan=280 @testsuite=869
Feature: ListProductionsInProdStoreUI
A creative worker is able to see a list of all Productions within a ProductionStore.

@testcase=956 @bvt @priority=1 @version = 3
Scenario: Verify productions are listed for a production store
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store in "Region1" region
Then Verify all productions are listed for "ProductionStore1" production store in "asc" order of "name"
And UI should include production details with below columns
| columns  |
| Name     |
| Created  |
| Modified |
| Size     |
| Status   |

@testcase=957 @priority=2 @NotApplicable @version = 3 @ignore
Scenario: Verify clicking the filter button the filter bar appears
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store in "Region1" region
When I click on "Filter Production" button
And I enter "{text}" into the "Filter Name" text field
Then I verify result includes "{text}" in "Prodcution Name column"


 #=================To be implemented ========================

#@testcase=958 @priority=2 @manual @NotApplicable @version = 2
#Scenario: Verify clicking the refresh the page refreshes
#	Given I have logged into Storage Automation portal using "admin" user
#	When I click on "Refresh" button
#	Then Verify the grid is refreshed
#
#@testcase=959 @priority=2 @manual @version=2
#Scenario: Verify contextual menu for offline/online production
#	Given I have logged into Storage Automation portal using "admin" user
#	When I filter "ProductionOnline" for "Filter by name" field
#	Then Verify the contextual menu has following options
#		| Context Menu |
#		| Make Online  |
#		| Delete       |
#		| Rename       |
#	When I filter "ProductionOFfline" for "Filter by name" field
#	Then Verify the contextual menu has following options
#		| Context Menu |
#		| Make Offline |
#		| Delete       |
#		| Rename       |