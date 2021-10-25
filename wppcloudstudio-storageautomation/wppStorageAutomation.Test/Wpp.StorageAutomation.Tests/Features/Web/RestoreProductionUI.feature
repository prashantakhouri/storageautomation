@ui @owner=Sumanth.Gogineni @ui @testplan=280 @testsuite=868 @parallel=false
Feature: RestoreProductionUI
A creative worker, production manager or administrator is able to restore a production

@testcase=954 @bvt @priority=1 @version=7 @ArchiveSingleProductionStore
Scenario: Verify production restore from UI
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store
When a production in the "ProductionStore1" is in "Offline" status
And I click on "More Options" button
And I click on "Make Online" button
Then Verify "Restoring" spinner is visible if action takes time
And production is restored and status is updated to Online in UI
And status is updated to "Online" in database
#And I verify that all folders and sub folders under a production are restored successfully from UI

@testcase=955 @priority=2 @version=5
Scenario: Verify Make offline option should be available when production is in Online status
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store
When a production in the "ProductionStore1" is in "Online" status
And I click on "More Options" button
Then I should see "Make Offline" option in the Contextual Menu

@testcase=994 @priority=2 @DeleteProduction @version=6
Scenario: Verify error message is displayed when user tried to restore a production which does not exists in archive storage account
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore2" production store
And I click on "Create Production" button
And I am redirected to "Create Production" page
When I enter "!!!_Production_!!!" into the "Production Name" text field
And I click on "Save" button
And I get a "!!!_Production_!!! created successfully." banner mesaage
And I click on "Discard" button
And a production in the "ProductionStore2" is in "Offline" status
And delete production from archive container
And I click on "More Options" button
And I click on "Make Online" button
Then user should see error message as "Restore failed. Production does not exist in Archive."

@testcase=3296 @priority=1 @testplan=280 @testsuite=2414 @version=2
Scenario: Verify latest version of files are restored when production is restored from UI
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store
When I upload "DummyTestFile.txt" file in "ProductionStore1_Production1" path under "ProductionStore1" in WIP storage
And I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
And I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
And I verify that newly uploaded files under "ProductionStore1" Production store are Archived
And I have deleted "DummyTestFile.txt" file in "ProductionStore1_Production1" path in WIP storage
And a "ProductionStore1_Production1" in the "ProductionStore1" is in "Offline" status
And I click on "More Options" button for production
And I click on "Make Online" button
Then production is restored and status is updated to Online in UI for production
And status is updated to "Online" in database
And "DummyTestFile.txt" file "ProductionStore1_Production1" path under "ProductionStore1" should not be restored
