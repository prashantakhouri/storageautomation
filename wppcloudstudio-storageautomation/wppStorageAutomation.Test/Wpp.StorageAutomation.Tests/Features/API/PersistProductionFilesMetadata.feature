@api @owner=saloni.shrivastava @testplan=280 @testsuite=870 @parallel=false
Feature: PersistProductionFilesMetadata
Ensure metadata associated with files within a production is preserved in in the Archive storage
so that it can be retrieved when a Production is restored to WIP.

@testcase=963 @bvt @priority=1 @version=2
Scenario: Verify metadata file is created at production level on archiving a production store
When I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
Then I verify if the API execution is "Completed" with in 4 min
And Verify all productions in the "ProductionStore1" Production Store have respective metadata file

@testcase=964 @bvt @priority=1 @version=2
Scenario: Verify metadata file is created at production level on archiving all production store
When I send the "Archive All Productionstores" POST request
Then I verify if the API execution is "Completed" with in 4 min
And Verify all productions in the all Production Stores have respective metadata file

@testcase=965 @priority=1 @version=2
Scenario: Verify metadata file content
When I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
Then Verify "ProductionStore1_Production1" Production contains the metadata file
And Verify contents of metadata file

@testcase=966 @priority=1 @version=3
Scenario: Verify empty folders are restored
Given I get the list of files and folders before making "ProductionStore1_Production1" production offline
And I send "Make Production Offline" API to restore "ProductionStore1_Production1" Production for "ProductionStore1" production store
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
When I send "Restore Production" API to restore "ProductionStore1_Production1" Production for "ProductionStore1" production store
And I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
Then I verify that all folders and sub folders under "ProductionStore1_Production1" are restored

@testcase=967 @priority=2 @version=3
Scenario: Verify created and Last Write Time of new uploaded file
Given I upload "DummyTestFile.txt" file in "ProductionStore1_Production1" path under "ProductionStore1" in WIP storage
And I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
And Verify "ProductionStore1_Production1" Production contains the metadata file
And Verify contents of metadata file
And I get the list of files and folders before making "ProductionStore1_Production1" production offline
And I send "Make Production Offline" API to restore "ProductionStore1_Production1" Production for "ProductionStore1" production store
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
When I send "Restore Production" API to restore "ProductionStore1_Production1" Production for "ProductionStore1" production store
And I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
Then I verify "DummyTestFile.txt" file and "ProductionStore1_Production1" production has updated created time

