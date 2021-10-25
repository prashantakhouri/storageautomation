@api @owner=Sumanth.Gogineni @testplan=280 @testsuite=1286 @parallel=false
Feature: CreateAProductionStore
In order to create the db entry, and it is a prerequisite
that the Storage accounts had already been created by another process

@testcase=1291 @bvt @priority=1 @DeleteParticularProductionStore @version=5
Scenario: Verify authorized user is able to register a production store in the database
	Given I am authenticated as "admin"
	And I delete existing "testcreateproductionstore" productionstore from database
	When I send the "Create a Production store" API with below data
		| Key  | Value                     |
		| Name | testcreateproductionstore |
	Then I receive valid HTTP response code "200"
	And I receive a sync response for 'Create a Production store' API
	And production store data is registered in database

@testcase=1292 @priority=2 @version=4
Scenario: Verify user should not be able to register same production store in the database
	Given I am authenticated as "admin"
	When I send the "Create a Production store" POST request with duplicate production store
		| Key  | Value                |
		| Name | testproductionstorea |
	Then Verify "failed" sync response with "422-UnprocessableEntity" for "statusCode"

@testcase=1293 @priority=2 @version=4 @ignore @invalidTestcase
Scenario: Verify error is thrown when user tries to register a production store which is not exists in WIP and ARC storage accounts
	Given I am authenticated as "admin"
	When I send the "Create a Production store" POST request with production store which is not exist in the WIP and ARC storage accounts
		| Key    | Value                     |
		| Name   | testcreateproductionstore |
		| WIPURL | /DummyProductionStore     |
	Then I receive valid HTTP response code "400"

@testcase=1294 @priority=2 @version=3
Scenario: Verify error is thrown when unauthorized user is able to register a production store in the database
	Given I am authenticated as "unauthorized" user
	When I send the "Create a Production store" API with below data
		| Key  | Value                     |
		| Name | testcreateproductionstore |
	Then I receive valid HTTP response code "401"