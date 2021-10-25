@api @owner=Aryamol.Jacob @testplan=280 @testsuite=3650 @parallel=false @manual
Feature: DeleteAProduction

@testcase=3652 @deleteproduction @priority=1 @bvt
Scenario: Verify if Manager is able to delete a Production
When I send "Delete Production" API to delete "ProductionStore3_Production1" Production for "ProductionStore3" production store as a Manager user
Then I receive valid HTTP response code "200"
And I verify if the API execution is "Completed" with in 60 sec
And I verify the latest version of Production is Archived if last archive time was before last modified time
And Verify Production "ProductionStore3_Production1" is deleted from "ProductionStore3" production store in WIP in "5" seconds
And Verify Production "ProductionStore3_Production1" is deleted from "ProductionStore3" production store in Archive storage in "5" seconds
And I verify that "ProductionStore3_Production1" is marked as "offline"

@testcase=3653 @deleteproduction @priority=3
Scenario: Verify that Delete Production API fails if Invalid Production store Id or Production name is sent
When I send "Delete Production" API to delete "InvalidProduction" Production for "ProductionStore3" production store as a Manager user
Then I receive valid HTTP response code "404"
When I send "Delete Production" API to delete "ProductionStore3_Production1" Production for "InvalidProductionStore" production store as a Manager user
Then I receive valid HTTP response code "404"

@testcase=3654 @deleteproduction @priority=2
Scenario: Verify that Non-Manager user is not able to delete a Production
When I send "Delete Production" API to delete "ProductionStore3_Production1" Production for "ProductionStore3" production store as a Non-Manager user
Then I receive valid HTTP response code "403"

@testcase=3655 @deleteproduction @priority=3
Scenario: Verify that Delete Production API fails if production is online
When I send "Delete Production" API to delete "OnlineProduction" Production for "ProductionStore3" production store as a Manager user
Then I receive valid HTTP response code "404"
