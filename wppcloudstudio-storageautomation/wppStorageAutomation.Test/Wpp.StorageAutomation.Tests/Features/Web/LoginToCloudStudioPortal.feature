@ui @owner=Sumanth.Gogineni @testplan=280 @testsuite=866
Feature: LoginToCloudStudioPortal
As a creative worker, I need to login to the Cloud Studio
Portal so that I am authorized to use its functions

@testcase=952 @bvt @priority=1 @version=2
Scenario: Verify authenticated users can be logged into cloud studio portal
	When I have logged into Storage Automation portal using "admin" user
	Then user is navigated to home page

@testcase=953 @bvt @priority=2 @version=2
Scenario: Verify unauthenticated users should not be logged into cloud studio portal
	When I have logged into Storage Automation portal using "unauthorized" user
	Then user can see error message as "Sign in failed!"
	And user should not be logged into portal

@testcase=1206 @priority=2
Scenario: Verify authenticated users can logged out from cloud studio portal
	When I have logged into Storage Automation portal using "admin" user
	And user is navigated to home page
	And I click on "Signout" button
	Then user is logged out from cloud studio portal