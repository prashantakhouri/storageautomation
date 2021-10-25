@ui @owner=Aryamol.Jacob @testplan=280 @parallel=false @manual @testsuite=371
Feature: FilterAndSortProductions

@testcase=3714 @bvt @priority=1 @filterproductions @testsuite=3711 @verison=2
Scenario: Verify if user is able to filter production by Name
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store in "Region1" region
And I click on "Filter Production" button
When I enter "ProductionStore1_Production1" into the "Filter Name" text field
Then Verify "ProductionStore1_Production1" is present in "Name" column
Then Verify "ProductionStore1_Production2" is not present in "Name" column

@testcase=3715 @bvt @priority=1 @sortproductions @testsuite=3712
Scenario: Verify if user is able to sort productions
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store in "Region1" region
And I click on column header and I verify the table is sorted
| Column   |
| Created  |
| Name     |
| Modified |
| Size     |
| Status   |