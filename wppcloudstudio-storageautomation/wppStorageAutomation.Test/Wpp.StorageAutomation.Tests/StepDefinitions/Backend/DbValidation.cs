// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbValidation.cs" company="Microsoft">
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//    OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//    OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Wpp.StorageAutomation.Tests.StepDefinitions
{
    using System;
    using System.Threading.Tasks;

    using Bdd.Core.Utils;

    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    using Wpp.StorageAutomation.Tests.Core;
    using Wpp.StorageAutomation.Tests.Entities;
    using Wpp.StorageAutomation.Tests.Executors;

    [Binding]
    public class DbValidation : WppApiStepDefinitionBase
    {
        [When(@"I get the Last archived time of ""(.*)""")]
        public async Task WhenIGetTheLastArchivedTimeOfAsync(string prod)
        {
            string production = this.WipStorageSettings[prod];
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetails, args: new { name = production }).ConfigureAwait(false);
            this.ScenarioContext["LastSyncDateTime"] = productionDetails[Constants.LastSyncDateTime];
        }

        [Then(@"I verify is the Last sync Time is updated for ""(.*)""")]
        public async Task ThenIVerifyIsTheLastSyncTimeIsUpdatedForAsync(string prod)
        {
            FileShareConnector fs = await this.CreateWIPConnectorAsync().ConfigureAwait(false);
            string production = fs.WipStorageSettings[prod];
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetails, args: new { name = production }).ConfigureAwait(false);
            var newLastSyncTime = productionDetails[Constants.LastSyncDateTime].ToDateTime();
            Assert.IsTrue(this.ScenarioContext["LastSyncDateTime"].ToDateTime() < newLastSyncTime && Math.Abs((newLastSyncTime - DateTime.UtcNow).TotalMinutes) <= 3, $"Scheduled archive failed: old Last Sync time- {this.ScenarioContext[$"LastSyncDateTime"]}, New last sync Time -{newLastSyncTime}, UTC Now -{DateTime.UtcNow}");
        }

        [Then(@"the response should be perisisted in the sql database with id, last sync time, offline status")]
        public async Task ThenTheResponseShouldBePerisistedInTheSqlDatabaseWithIdLastSyncTimeOfflineStatus()
        {
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetails, args: new { name = this.ScenarioContext["ProductionDirName"] }).ConfigureAwait(false);
            Assert.IsTrue(productionDetails.ContainsKey(Constants.Id));
            Assert.IsTrue(productionDetails.ContainsKey(Constants.LastSyncDateTime));
            Assert.IsTrue(productionDetails.ContainsKey(Constants.Status));
            Assert.IsNotNull(productionDetails);
            Assert.IsNotNull(productionDetails[Constants.ProductionStoreId]);
            Assert.AreEqual("/" + this.ScenarioContext[Constants.ProductionStoreName] + "/" + this.ScenarioContext["ProductionDirName"], productionDetails[Constants.WIPUrl]);
            Assert.AreEqual(Constants.Online, productionDetails[Constants.Status]);
            JObject apiStatus = (JObject)this.ScenarioContext[Constants.GetApiResult];
            this.ScenarioContext[Constants.CreatedDateTime] = apiStatus["createdDateTime"];
            Assert.IsTrue(productionDetails[Constants.CreatedDateTime].ToString().Equals(this.ScenarioContext[Constants.CreatedDateTime].ToString()), $"Production Id - {productionDetails[Constants.Id]} \n Created Time - {this.ScenarioContext[Constants.CreatedDateTime]} \n Api Request Triggered Time - {this.ScenarioContext[Constants.CreatedDateTime]}");
            Assert.IsNull(productionDetails[Constants.LastSyncDateTime]);
        }

        [Then(@"Verify response should be perisisted in the sql database with id, last sync time, offline status")]
        public async Task ThenVerifyResponseShouldBePerisistedInTheSqlDatabaseWithIdLastSyncTimeOfflineStatus()
        {
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetails, args: new { name = this.ScenarioContext["ProductionDirName"] }).ConfigureAwait(false);
            Assert.IsTrue(productionDetails.ContainsKey(Constants.Id));
            Assert.IsTrue(productionDetails.ContainsKey(Constants.LastSyncDateTime));
            Assert.IsTrue(productionDetails.ContainsKey(Constants.Status));
            Assert.IsNotNull(productionDetails);
            Assert.IsNotNull(productionDetails[Constants.ProductionStoreId]);
            Assert.AreEqual("/" + this.ScenarioContext[Constants.ProductionStoreName] + "/" + this.ScenarioContext["ProductionDirName"], productionDetails[Constants.WIPUrl]);
            Assert.AreEqual("Online", productionDetails[Constants.Status]);
            Assert.IsNull(productionDetails[Constants.LastSyncDateTime]);
        }

        [Then(@"record with particular production name should not be created in the database")]
        [Then(@"last sync date and time of the record with particular production name should not be updated in the database")]
        public async Task ThenRecordWithParticularProductionNameShouldNotBeCreatedInTheDatabase()
        {
            var tableDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetails, args: new { name = Constants.InvalidProductionName }).ConfigureAwait(false);
            //// As production store with "ProdWith *" will not be stored in the database so tableDetails will ge null value
            Assert.IsNull(tableDetails);
        }

        [Then(@"the last sync date and time should be perisisted in the database")]
        public async Task ThenTheLastSyncDateAndTimeShouldBePerisistedInTheDatabase()
        {
            JObject apiStatus = (JObject)this.ScenarioContext[Constants.ApiStatus];
            this.ScenarioContext[Constants.LastSyncDateTime] = apiStatus["output"]["Data"][0]["ArchiveDate"];
            var productionDetails = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetProductionDetailsByProductionStore, args: new { productionStoreId = this.ScenarioContext["ProductionStoreName"] }).ConfigureAwait(false);
            foreach (var productionItem in productionDetails)
            {
                var lastSyncDateTime = productionItem[Constants.LastSyncDateTime];
                Assert.IsTrue(lastSyncDateTime.ToString().Equals(this.ScenarioContext[Constants.LastSyncDateTime].ToString()), $"Production Id - {productionItem[Constants.Id]} \n Archive Complete Time - {this.ScenarioContext[Constants.LastSyncDateTime]} \n Last Sync Date - {lastSyncDateTime}");
            }
        }

        [Then(@"the last sync date and time should be perisisted in the database for all production stores")]
        public async Task ThenTheLastSyncDateAndTimeShouldBePerisistedInTheDatabaseForAllProductionStores()
        {
            JObject apiStatus = (JObject)this.ScenarioContext[Constants.ApiStatus];
            this.ScenarioContext[Constants.LastSyncDateTime] = apiStatus["output"]["Data"][0]["ArchiveDate"];
            var productionDetails = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetAllProductionDetails).ConfigureAwait(false);
            foreach (var productionItem in productionDetails)
            {
                var lastSyncDateTime = productionItem[Constants.LastSyncDateTime];
                var dateCompareOfRequestTriggeredAndLastSync = DateTime.Compare(Convert.ToDateTime(this.ScenarioContext[Constants.RequestTriggeredDateTime]), Convert.ToDateTime(lastSyncDateTime));
                var dateCompareOfLastSyncAndArchiveCompleted = 1.0;
                if (!lastSyncDateTime.ToString().Equals(this.ScenarioContext[Constants.LastSyncDateTime].ToString()))
                {
                    dateCompareOfLastSyncAndArchiveCompleted = Convert.ToDateTime(lastSyncDateTime).Subtract(Convert.ToDateTime(this.ScenarioContext[Constants.LastSyncDateTime])).TotalSeconds;

                        // DateTime.Compare(Convert.ToDateTime(lastSyncDateTime), Convert.ToDateTime(this.ScenarioContext[Constants.LastSyncDateTime]));
                }
                else
                {
                    dateCompareOfLastSyncAndArchiveCompleted = 0;
                }

                Assert.IsTrue(dateCompareOfRequestTriggeredAndLastSync <= 0 && Math.Abs(dateCompareOfLastSyncAndArchiveCompleted) <= 5, $"Production Id - {productionItem[Constants.Id]} \n Triggered Time - {this.ScenarioContext[Constants.RequestTriggeredDateTime]} \n LastSync Time stored in DB - {lastSyncDateTime} \n Archive Completed Time - {this.ScenarioContext[Constants.LastSyncDateTime]}");
            }
        }

        [Then(@"status is updated to ""(.*)"" in database")]
        public async Task ThenStatusIsUpdatedToOnlineInDatabase(string status)
        {
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetails, args: new { name = this.ScenarioContext[Constants.ProductionDirName] }).ConfigureAwait(false);
            Assert.AreEqual(status, productionDetails[Constants.Status]);
        }

        [Then(@"I verify that ""(.*)"" is marked as ""(.*)""")]
        public async Task ThenIVerifyThatIsMarkedAs(string productionName, string status)
        {
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetails, args: new { name = this.WipStorageSettings[productionName] }).ConfigureAwait(false);
            Assert.IsTrue(productionDetails[Constants.Status].EndsWithIgnoreCase(status), $"Production status is not {status}");
        }

        [Then(@"I verify the latest version of Production is Archived if last archive time was before last modified time")]
        public async Task ThenIVerifyTheLatestVersionOfProductionIsArchivedIfLastArchiveTimeWasBeforeLastModifiedTime()
        {
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetails, args: new { name = this.ScenarioContext["ProductionDirName"] }).ConfigureAwait(false);
            Console.WriteLine(productionDetails[Constants.LastSyncDateTime]);

            // get last modified time from metadata file
            // verify its less than last update
            // or upload or edit a file and verify if its archived
        }

        [Then(@"production store data is registered in database")]
        public async Task ThenProductionStoreDataIsRegisteredInDatabaseAsync()
        {
            JObject apiStatus = (JObject)this.ScenarioContext[Constants.Result];
            Assert.IsNotNull(apiStatus, $"Null Response Body for Register API");
            Assert.IsNotNull(apiStatus["data"][0]["id"], $"Id Token nout found in response body{apiStatus.ToString()}");
            var productionStoreId = apiStatus["data"][0]["id"];
            var productionStoreName = apiStatus["data"][0]["name"];
            this.ScenarioContext[Constants.ProductionStoreName] = productionStoreName;
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetProductionStoreDetails, args: new { name = "testcreateproductionstore" }).ConfigureAwait(false);
            Assert.AreEqual(productionStoreId.ToString(), productionDetails[Constants.Id]);
            Assert.AreEqual(productionStoreName.ToString(), productionDetails[Constants.Name]);
            Assert.AreEqual("/testcreateproductionstore", productionDetails[Constants.WIPUrl.ToUpper()]);
            Assert.AreEqual("/testcreateproductionstore", productionDetails[Constants.ArchiveURL]);
            Assert.AreEqual("Asia", productionDetails[Constants.Region]);
            Assert.AreEqual(Constants.ProductionStoreSize, productionDetails[Constants.WIPAllocatedSize]);
            Assert.AreEqual(Constants.ProductionStoreSize, productionDetails[Constants.ArchiveAllocatedSize]);
            Assert.AreEqual(this.StorageAccountDetails["sa1_WIP_Key"], productionDetails[Constants.WIPKeyName]);
            Assert.AreEqual(this.StorageAccountDetails["sa1_Arc_Key"], productionDetails[Constants.ArchiveKeyName]);
        }

        [Then(@"the value of ""(.*)"" persists in the database")]
        public async Task ThenTheValueOfPersistsInTheDatabase(string columnName)
        {
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetProductionStoreDetails, args: new { name = "testcreateproductionstore" }).ConfigureAwait(false);
            if (columnName.ContainsIgnoreCase("wip"))
            {
                Assert.AreEqual(this.StorageAccountDetails["sa1_WIP_Key"], productionDetails[Constants.WIPKeyName]);
            }
            else if (columnName.ContainsIgnoreCase("arc"))
            {
                Assert.AreEqual(this.StorageAccountDetails["sa1_Arc_Key"], productionDetails[Constants.ArchiveKeyName]);
            }
        }
    }
}
