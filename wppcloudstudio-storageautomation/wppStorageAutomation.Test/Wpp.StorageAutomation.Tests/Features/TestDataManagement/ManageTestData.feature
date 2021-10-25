@owner=saloni.shrivastava
Feature: ManageTestData
	Sceanrios to move test data to WIP and to clean up the test data from WIP and Archive


@copytestdata @owner=arjaco
Scenario: Copy Test Data to WIP from Test SA
	Then Copy Test Data to WIP from Test SA

@cleanup
Scenario: Cleanup WIP storage account
	Then Clear WIP Storage Account

@cleanup
Scenario: Cleanup Archive storage account
	Then Clear Archive Storage Account

#@copytestdata @ignore
#Scenario: Move production from Blob to Blob
#	Then Move Test Data

@Token @ui
Scenario: Generate Acess Token
	Then I have generated Access Token for "admin"