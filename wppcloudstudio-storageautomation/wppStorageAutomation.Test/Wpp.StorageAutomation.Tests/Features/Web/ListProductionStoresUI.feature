@ui @owner=saloni.shrivastava @testplan=280 @testsuite=1287
Feature: ListProductionStoresUI
A creative worker is able to see a list of all Production Stores.

@testcase=1789 @bvt @priority=1 @version=2
Scenario: Verify production stores are listed on left panel grouped by region
Given I have logged into Storage Automation portal using "admin" user
Then Verify regions are listed in "asc" order

@testcase=1794 @bvt @priority=1 @version=2
Scenario: Verify productions are listed for first production store in first region
Given I have logged into Storage Automation portal using "admin" user
Then Verify first production store is selected in the production store list

@testcase=1790 @bvt @priority=1 @version=3
Scenario: Verify regions can be expanded and collapsed
Given I have logged into Storage Automation portal using "admin" user
When I "expand" region with the name "Region1"
Then Verify production stores "are" listed for "Region1" region
When I "collapse" region with the name "Region1"
Then Verify production stores "not" listed for "Region1" region

@testcase=1791 @priority=1 @version=4
Scenario: Verify navigation to production store from Create Production Page
Given I have logged into Storage Automation portal using "admin" user
And I click on "Create Production" button
When I navigate to "ProductionStore3" production store in "Region1" region
Then Verify all productions are listed for "ProductionStore3" production store in "asc" order of "name"
