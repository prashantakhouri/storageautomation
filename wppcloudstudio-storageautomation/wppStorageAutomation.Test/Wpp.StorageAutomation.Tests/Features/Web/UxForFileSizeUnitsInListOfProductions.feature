@ui @owner=Sumanth.Gogineni @testplan=280 @testsuite=3592 @parallel=false @manual
Feature: UxForFileSizeUnitsInListOfProductions
As a user I need to see the size of a production in megabytes,
gigabytes or terabytes so that I can interpret the size more easily


@testcase=3631 @createproduction @bvt @DeleteProduction @priority=1
Scenario: Verify size of the production should display meanigful value
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store in "Region1" region
And I click on "Create Production" button
And I am redirected to "Create Production" page
When I enter "!!Test-Prod_UI!!" into the "Production Name" text field
And I click on "Save" button
Then I verify "Save" is disabled
And Verify Production "!!Test-Prod_UI!!" is created in "ProductionStore1" production store in "10" seconds
And Verify Production "!!Test-Prod_UI!!" is created in "ProductionStore1" Production Store on UI
When production size is "1000" bytes
Then size column in grid should display as "1000 B"
When production size is "1024" bytes
Then size column in grid should display as "1 KB"
When production size is "1048576" bytes
Then size column in grid should display as "1 MB"
When production size is "1073741824" bytes
Then size column in grid should display as "1 GB"
When production size is "1099511627776" bytes
Then size column in grid should display as "1 TB"
