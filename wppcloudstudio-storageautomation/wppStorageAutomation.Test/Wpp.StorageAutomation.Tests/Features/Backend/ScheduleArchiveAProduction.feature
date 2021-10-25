@backend @owner=aryamol.jacob @testplan=280 @testsuite=867 @parallel=false
Feature: ScheduleArchiveAProduction
Schedule the Arching to happen every 15 min

@testcase=992 @archiveschedule @priority=1 @bvt @version=2
Scenario: Verify that Archiving of production stores are scheduled to happen every 15 min
When I get the Last archived time of "ProductionStore1_Production1"
And I upload "DummyTestFile2.txt" file in "ProductionStore1_Production1" path under "ProductionStore1" in WIP storage
Then I wait till "15" min from last archive time and additional "1" min for the archive to complete
And I verify that all folders and sub folders under "ProductionStore1" Production store are archived
And I verify is the Last sync Time is updated for "ProductionStore1_Production1"
And I verify is the Last sync Time is updated for "ProductionStore2_EmptyProduction"

#==========Not implemented ===========#
#@archiveschedule @priority=2 @manual
#Scenario: Verify that if an archiving is in progress the newly triggered archive does not trgirrer copying of data
#When I get the Last archived time of "ProductionStoreLarge"
#Then I verify that the Archive is triggered 15 min after last archive
#And I verify that archive is still in progress after 15 min
#And I verify that new Archive triggered returns warning "Archiving already in progress"
#And I verify the copying of data is not happening again
#And last sync date and time of the record with particular production name should not be updated in the database as per second API
