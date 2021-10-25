@api @owner=saloni.shrivastava @testplan=280 @testsuite=284 @parallel=false
Feature: RestoreProduction
Copy an offline Production from Archive to the WIP Storage


@testcase=471 @restore @priority=1 @bvt @version=8
Scenario: 02 Verify all folders are copied from archive to wip
Given I get the list of files and folders before making "ProductionStore3_Production1" production offline
And I send "Make Production Offline" API to restore "ProductionStore3_Production1" Production for "ProductionStore3" production store
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
And I verify the latest version of Production is Archived if last archive time was before last modified time
When I send "Restore Production" API to restore "ProductionStore3_Production1" Production for "ProductionStore3" production store
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
And Verify Production "ProductionStore3_Production1" is restored in "ProductionStore3" production store in "5" seconds
And I verify that all folders and sub folders under "ProductionStore3_Production1" are restored

@testcase=472 @restore @priority=1 @version=9
Scenario: 03 Verify production not restored from archive if status is online in DB
When I send "Restore Production" API to restore "ProductionStore3_Production1" Production for "ProductionStore3" production store
And I verify if the API execution is "Completed" with in 60 sec
And Verify Production "ProductionStore3_Production1" is restored in "ProductionStore3" production store in "5" seconds
When I send "Restore Production" API to restore "ProductionStore3_Production1" Production for "ProductionStore3" production store
Then Verify "failed" response body for "Restore Production" API
| Key        | Value          |
| StatusCode | 400-BadRequest |

@testcase=2208 @restore @priority=1
Scenario: 04 Verify production not restored from archive if status is offline in DB and production exists in WIP
When I send "Restore Production" API to restore "ProductionStore3_Production1" Production for "ProductionStore3" production store
And I verify if the API execution is "Completed" with in 60 sec
And Verify Production "ProductionStore3_Production1" is restored in "ProductionStore3" production store in "5" seconds
And I set the status to "Offline" for "ProductionStore3_Production1" production
When I send "Restore Production" API to restore "ProductionStore3_Production1" Production for "ProductionStore3" production store
Then Verify "failed" response body for "Restore Production" API
| Key        | Value          |
| StatusCode | 400-BadRequest |

@testcase=473 @restore @priority=1 @version=9
Scenario: 01 Verify production not restored if wrong path
When I send "Restore Production" API to restore "ProductionStore3_Production1" Production for "WrongStore" production store
Then I receive valid HTTP response code "404"


@restore @priority=2 @manual @notautomatable @testsuite=4053
Scenario: Verify that partially restored productions are deleted from WIP if restore fails
Given I get the list of files and folders before making "ProductionStore3_Production1" production offline
And I send "Make Production Offline" API to restore "ProductionStore3_Production1" Production for "ProductionStore3" production store
When the restore started and the Production folder is created in WIP
And the API to restore fails
Then I verify that partially restored productions are deleted from WIP
And I verify the status of Production is retained as Offline
