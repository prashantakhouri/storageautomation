@ui @owner=Aryamol.Jacob @testplan=280 @testsuite=2599 @parallel=false
Feature: MakeProductionOfflineUI
Make production offline and Delete from WIP

@testcase=3076 @bvt @priority=1 @version=4 @RestoreProduction
Scenario: Verify production is make offline from UI
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store
When production in the "ProductionStore1" is in "Online" status
And I click on "More Options" button
And I click on "Make Offline" button
And I click "Ok" on confirmation popup
Then Verify "Making Offline" spinner is visible if action takes time
And production status is updated to "Offline" in UI
And status is updated to "Offline" in database
And I verify that production in "ProductionStore1" is deleted from WIP
And I verify that "<production> taken offline successfully." message is displayed