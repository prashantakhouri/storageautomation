@ui @owner=Aryamol.Jacob @testplan=280 @testsuite=3650 @parallel=false @manual
Feature: DeleteProductionUI

@testcase=3656 @bvt @priority=1 @deleteproduction @version=2
Scenario: Verify that Manager user is able to delete a production in Offline state
Given I have logged into Storage Automation portal using "Manager" user
And I navigate to "ProductionStore1" production store
When production in the "ProductionStore1" is in "Offline" status
And I click on "More Options" button
And I click on "Delete" button
And I click "Ok" on confirmation popup
Then I verify that "<production> is deleted successfully." message is displayed
And status is updated to "Deleted" in database

@testcase=3657 @priority=2 @deleteproduction @version=2
Scenario: Verify that Non-Manager user is not able to delete a production in Offline state
Given I have logged into Storage Automation portal using "Non-Manager" user
And I navigate to "ProductionStore1" production store
When production in the "ProductionStore1" is in "Offline" status
And I click on "More Options" button
Then I verify that "Delete" button is greyed out
