@ui @owner=Sumanth.Gogineni @testplan=280 @testsuite=3593 @parallel=false @manual
Feature: ErrorPage
In order to handle unhadled exceptions in the portal

@testcase=3658 @priority=1 @notautomatable @version=2
Scenario: Verify error message should be displayed when unhandled or network issues occurs
Given I have logged into Storage Automation portal using "admin" user
When unhandled exception or network error occurs
Then "Sorry, something went wrong" message should be displayed
