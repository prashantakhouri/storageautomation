@ui @owner=saloni.shrivastava @testplan=280 @testsuite=732 @parallel=false
Feature: CreateProductionUI
A creative worker is able to create a new production

@testcase=764 @createproduction @bvt @DeleteProduction @priority=1 @version=8
Scenario: Verify creation of a production from UI
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store in "Region1" region
And I click on "Create Production" button
And I am redirected to "Create Production" page
When I enter "Test-Prod_UI" into the "Production Name" text field
And I click on "Save" button
Then I verify "Save" is disabled
And Verify Production "Test-Prod_UI" is created in "ProductionStore1" production store in "10" seconds
And Verify Production "Test-Prod_UI" is created in "ProductionStore1" Production Store on UI
And Verify response should be perisisted in the sql database with id, last sync time, offline status

@testcase=765 @createproduction @priority=2 @version=7
Scenario: Verify message when Create Production button is clicked with no production name
	Given I have logged into Storage Automation portal using "admin" user
	And I navigate to "ProductionStore1" production store in "Region1" region
	And I click on "Create Production" button
	And I am redirected to "Create Production" page
	When I enter " " into the "Production Name" text field
	Then I verify "Save" is disabled
	And I get a empty field message for "Production Name" field

@testcase=766 @createproduction @priority=2 @version=7
Scenario Outline: Verify message when Create Production button is clicked with invalid production name
	Given I have logged into Storage Automation portal using "admin" user
	And I navigate to "ProductionStore1" production store in "Region1" region
	And I click on "Create Production" button
	And I am redirected to "Create Production" page
	When I enter "<Production name>" into the "Production Name" text field
	Then I verify "Save" is disabled
	And I get a field message "Only regular characters can be used for a name, please avoid using special characters like '\ / : | < > * ?'" for "Production Name" field

	Examples:
		| Production name |
		| ProdWith \      |
		| ProdWith *      |
		| ProdWith /      |
		| ProdWith \|     |
		| ProdWith :      |
		| ProdWith <      |
		| ProdWith >      |
		| ProdWith ?      |

@testcase=767 @createproduction @priority=2 @DeleteProduction @version=8
Scenario Outline: Verify duplicate production creation is not allowed
Given I have logged into Storage Automation portal using "admin" user
And I navigate to "ProductionStore1" production store in "Region1" region
And I click on "Create Production" button
And I am redirected to "Create Production" page
And I enter "Test-Prod_UI" into the "Production Name" text field
And I click on "Save" button
And Verify Production "Test-Prod_UI" is created in "ProductionStore1" production store in "10" seconds
And Verify Production "Test-Prod_UI" is created in "ProductionStore1" Production Store on UI
When I click on "Create Production" button
And I enter "<Production name>" into the "Production Name" text field
And I click on "Save" button
Then I get a "The name provided has already been used, please give a different name." banner mesaage
Examples:
| Production name |
| Test-Prod_UI    |
| tEST-pROD_ui    |

@testcase=768 @createproduction @priority=2 @version=7
Scenario: Verify production name length should not be more than 255 characters
	Given I have logged into Storage Automation portal using "admin" user
	And I navigate to "ProductionStore1" production store in "Region1" region
	And I click on "Create Production" button
	And I am redirected to "Create Production" page
	When I enter "QVHphwBH8sS5tM6ZhPb5WhD9Q10hS8xiC1856hnPedXWRP22iTYKErTM7MxFyJn7C3RSPmQbAsxtxyjLes3pAtaXfbosTZiDKDDw6fHIIoPzz3WDCuGhGBfbDD3altjW1OtCSCLs7KfAbO9ZJgpdN76lIQVVO610uC4y3wVxOJgTYQXQfuy7lqgXw9rLMNIM0CHb3n5eHmdCNyerugFE7DmV5pnML3E61gusfsGxRKSC6bneBY6iQtVvThBJlR0mUs4" into the "Production Name" text field
	Then I get a field message "Maximum character limit is 255." for "Production Name" field

@testcase=920 @createproduction @priority=2 @verison=8
Scenario: Verify production name length should not be less than 8 characters
	Given I have logged into Storage Automation portal using "admin" user
	And I navigate to "ProductionStore1" production store in "Region1" region
	And I click on "Create Production" button
	And I am redirected to "Create Production" page
	When I enter "Test8" into the "Production Name" text field
	Then I verify "Save" is disabled
	And I get a field message "Minimum character limit is 8." for "Production Name" field

@testcase=769 @createproduction @priority=2 @version=7
Scenario: Verify Production is not created on clicking on Discard in Create Production Screen
	Given I have logged into Storage Automation portal using "admin" user
	And I navigate to "ProductionStore1" production store in "Region1" region
	And I click on "Create Production" button
	And I am redirected to "Create Production" page
	When I enter "Test_Discard" into the "Production Name" text field
	And I click on "Discard" button
	Then I am redirected to "List Production" page