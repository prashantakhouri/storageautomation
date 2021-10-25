@api @owner=Aryamol.Jacob @testplan=280 @testsuite=2598 @parallel=false
Feature: MakeAProductionOffline
Make production offline and Delete from WIP

@testcase=3077 @makeoffline @priority=1 @bvt @RestoreProduction @version=2
Scenario: Verify making a production offline
When I send "Make Production Offline" API to restore "ProductionStore3_Production1" Production for "ProductionStore3" production store
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
And I verify the latest version of Production is Archived if last archive time was before last modified time
And Verify Production "ProductionStore3_Production1" is deleted from "ProductionStore3" production store in WIP in "5" seconds
And I verify that "ProductionStore3_Production1" is marked as "offline"

@testcase=3078 @makeoffline @priority=1 @bvt @ verison=2
Scenario: Verify that Make offline API fails if Invalid Production store Id or Production name is sent
When I send "Make Production Offline" API to restore "InvalidProduction" Production for "ProductionStore3" production store
Then I receive valid HTTP response code "404"
When I send "Make Production Offline" API to restore "ProductionStore3_Production1" Production for "InvalidProductionStore" production store
Then I receive valid HTTP response code "404"

