@api @owner=aryamol.jacob @testplan=280 @testsuite=283 @parallel=false
Feature: ArchiveAProduction
Copy an online Production to Archive, ensuring that Production directory structures and
valuable metadata of the directories and files that it contains are preserved

# ================= Test cases related to productionstore/archive API=============================#
@testcase=430 @archive @priority=1 @bvt @version=5
Scenario: Verify that all folders subfolders and files from all Production stores are Archived
When I send the "Archive All Productionstores" POST request
Then I receive valid HTTP response code "200"
And Verify "Archiving" state for productions in DB
And I verify if the API execution is "Completed" with in 4 min
And I verify that all Productions and its files under all Production stores are archived

@testcase=481 @archive @priority=1 @bvt @version=4
Scenario: Verify that new files uploaded in all Production stores are Archived in the same folder structure
When I send the "Archive All Productionstores" POST request
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
And I verify that all Productions and its files under all Production stores are archived
When I upload "DummyTestFile.txt" file in "ProductionStore1_Production1" path under "ProductionStore1" in WIP storage
And I upload "DummyTestFile.txt" file in "ProductionStore1_Production1_SubDir1" path under "ProductionStore1" in WIP storage
Then I verify that newly uploaded files are archived

@testcase=433 @archive @priority=1 @bvt @version=4
Scenario: Verify the files of different format are Archived successfully from all Production stores
When I send the "Archive All Productionstores" POST request
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
And I verify that all files of below formats are Archived
| format |
| .txt   |
| .png   |

@testcase=432 @archive @priority=1 @bvt @version=4
Scenario: Verify that all empty folders and sub folders from all Production stores are archived
When I send the "Archive All Productionstores" POST request
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
And I verify that "ProductionStore2_EmptyProduction" is Archived and a hidden Json file is present in the Archived production

@testcase=789 @archive @priority=1 @bvt @testplan=280 @testsuite=468
Scenario:  Verify that latest version of files from all Production stores are Archived
When I upload "DummyTestFile.txt" file in "ProductionStore1_Production1" path under "ProductionStore1" in WIP storage
When I send the "Archive All Productionstores" POST request
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
Then I verify that newly uploaded files under "ProductionStore1" Production store are Archived
When I edit the content in "DummyTestFile.txt" file in "ProductionStore1_Production1" path in WIP storage
When I send the "Archive All Productionstores" POST request
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
Then I verify the content of "DummyTestFile.txt" file in "ProductionStore1_Production1" path in Archive storage

# ================= Test cases related to productionstore/{id} API=============================#
@testcase=482 @archive @priority=1 @bvt @testplan=280 @testsuite=468 @version=4
Scenario: Verify that all folders subfolders and files from a productionstore are Archived in the same folder structure
When I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
Then I receive valid HTTP response code "200"
And Verify "Archiving" state for productions in DB
And I verify if the API execution is "Completed" with in 4 min
And I verify that all folders and sub folders under "ProductionStore1" Production store are archived

@testcase=483 @archive @priority=1 @bvt @testplan=280 @testsuite=468 @version=3
Scenario: Verify the files of different format are Archived successfully from a productionstore
When I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
And I verify that all files of below formats under "ProductionStore1" are Archived
| format |
| .txt   |
| .png   |

@testcase=484 @archive @priority=1 @bvt @testplan=280 @testsuite=468 @version=3
Scenario: Verify that new files uploaded in a productionstore are Archived in the same folder structure
When I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
And I verify that all folders and sub folders under "ProductionStore1" Production store are archived
When I upload "DummyTestFile.txt" file in "ProductionStore1_Production1" path in WIP storage
And I upload "DummyTestFile.txt" file in "ProductionStore1_Production1_SubDir1" path in WIP storage
Then I verify that newly uploaded files under "ProductionStore1" Production store are Archived

@testcase=486 @archive @priority=1 @bvt @testplan=280 @testsuite=468 @version=4
Scenario: Verify that all empty folders and sub folders from a productionstore are archived
When I send the "Archive A Productionstore" POST request for "ProductionStore2" Productionstore
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
And I verify that "ProductionStore2_EmptyProduction" is Archived and a hidden Json file is present in the Archived production

@testcase=509 @archive @priority=1 @bvt @testplan=280 @testsuite=468 @version=6
Scenario: Verify that correct error message is displayed when an invalid production store name is passed in the Archive Production store API
When I send the "Archive A Productionstore" POST request for "invalidproductionstore" Productionstore
Then I receive valid HTTP response code "404"

@testcase=790 @archive @priority=1 @bvt @testplan=280 @testsuite=468
Scenario: Verify that latest version of files from given Production stores are Archived
When I upload "DummyTestFile.txt" file in "ProductionStore1_Production1" path under "ProductionStore1" in WIP storage
And I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
Then I verify that newly uploaded files under "ProductionStore1" Production store are Archived
When I edit the content in "DummyTestFile.txt" file in "ProductionStore1_Production1" path in WIP storage
And I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
Then I verify the content of "DummyTestFile.txt" file in "ProductionStore1_Production1" path in Archive storage
#--------------------------------Scenarios not covered in sprint1---------------------------
#Scenario: Verify when a folder or sub folder is remaned the same is updated in Archived folder
#Scenario: Verify when a file in a folder or sub folder is remaned the same is updated in Archived folder
#Scenario: Verify when a folder subfolder or a file is deleted from WIP same is deleted in Archived

@testcase=2488 @priority=1 @bvt @testplan=280 @testsuite=2414
Scenario: Verify deleted files in production store should be soft deleted in Archive container
When I upload "DummyTestFile.txt" file in "ProductionStore1_Production1" path under "ProductionStore1" in WIP storage
And I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
Then I verify that newly uploaded files under "ProductionStore1" Production store are Archived
When I have deleted "DummyTestFile.txt" file in "ProductionStore1_Production1" path in WIP storage
And I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
And I verify "DummyTestFile.txt" file in "ProductionStore1_Production1" is deleted from Archive container

@testcase=2489 @priority=2 @testplan=280 @testsuite=2414
Scenario: Verify deleted folders in production store should be soft deleted in Archive container
When I upload "DummyTestFolder" folder in "ProductionStore1_Production1" path under "ProductionStore1" in WIP storage
And I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
When I have deleted "DummyTestFolder" folder in "ProductionStore1_Production1" path in WIP storage
And I send the "Archive A Productionstore" POST request for "ProductionStore1" Productionstore
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 4 min
Then I verify "DummyTestFolder" folder in "ProductionStore1_Production1" is deleted from Archive container
