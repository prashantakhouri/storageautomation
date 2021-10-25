// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BeforeAndAfterSteps.cs" company="Microsoft">
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//    OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//    OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Wpp.StorageAutomation.Tests.Executors
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Bdd.Core;
    using Bdd.Core.Utils;

    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    using TechTalk.SpecFlow;
    using Wpp.StorageAutomation.Tests.Core;
    using Wpp.StorageAutomation.Tests.Entities;
    using Wpp.StorageAutomation.Tests.Properties;

    [Binding]
    public sealed class BeforeAndAfterSteps : WppApiStepDefinitionBase
    {
        [AfterScenario(@"CreateNewProductionInPS1")]
        public async Task CreateNewProductionInPS1()
        {
            string psName = this.WipStorageSettings["ProductionStore1"];
            string prod = $"TestProduction_{new Random().Next(1000)}";
            await this.CreateProdUsingAPIAsync(psName, prod);
            Assert.IsNotNull(this.Result, $"Create Production failed with Response code{this.ScenarioContext["ResponseMsg"]}");
        }

        [AfterScenario(@"DeleteParticularProductionStore")]
        public async Task DeleteProductionStore()
        {
            // Clear Production Store from DB
            await this.ReadSqlAsStringDictionaryAsync(SqlQueries.DeleteAProductionStore, args: new { name = this.ScenarioContext[Constants.ProductionStoreName].ToString() }).ConfigureAwait(false);
        }

        [AfterScenario(@"DeleteProduction")]
        public async Task DeleteProduction()
        {
            if (this.ScenarioContext.ContainsKey(Constants.ProductionDirName))
            {
                FileShareConnector fs = await this.CreateWIPConnectorAsync().ConfigureAwait(false);
                string prodName = this.ScenarioContext.Get<string>(Constants.ProductionDirName);
                string wipStoreName = this.ScenarioContext.Get<string>(Constants.ProductionStoreName);
                var productionStoreId = await this.GetPSGUIDAsync(wipStoreName);
                await fs.DeleteProduction(prodName, wipStoreName);

                // Clear Production from DB
                await this.ReadSqlAsStringDictionaryAsync(SqlQueries.DeleteProductionFromProductionStore, args: new { name = prodName, productionStoreId }).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("Production Name not set in Scenario Context");
            }
        }

        [AfterScenario(@"RestoreProduction")]
        public async Task RestoreProduction()
        {
            if (this.ScenarioContext.ContainsKey(Constants.ProductionDirName))
            {
                var prod = this.ScenarioContext[Constants.ProductionDirName].ToString();
                var ps = this.ScenarioContext[Constants.WIPStore].ToString();
                await this.RestoreProdUsingAPIAsync(prod, ps);
            }
            else
            {
                throw new Exception("Production Name not set in Scenario Context");
            }
        }

        [AfterScenario(@"DeleteMultipleProductions")]
        public async Task DeleteMultipleProduction()
        {
            await this.DeleteProductionUsingKey(this.ScenarioContext[Constants.MultipleSA1].ToString(), "productionstore-b-production").ConfigureAwait(false);
            await this.DeleteProductionUsingKey(this.ScenarioContext[Constants.MultipleSA2].ToString(), "productionstore-d-production").ConfigureAwait(false);
        }

        public async Task DeleteProductionUsingKey(string productionStore, string production)
        {
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetProductionStoreDetails, args: new { name = productionStore }).ConfigureAwait(false);
            FileShareConnector fs = new FileShareConnector(productionDetails[Constants.WIPKeyName]);
            var productionStoreId = await this.GetPSGUIDAsync(productionStore);
            await fs.DeleteProduction(production, productionStore);

            // Clear Production from DB
            await this.ReadSqlAsStringDictionaryAsync(SqlQueries.DeleteProductionFromProductionStore, args: new { name = production, productionStoreId }).ConfigureAwait(false);
        }

        [Then(@"Clear Archive Storage Account")]
        [AfterScenario(@"CleanArchive")]
        public async Task ThenClearArchiveStorageAccountAsync()
        {
            var productionStoreDetails = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetDistinctArchiveKeyNames).ConfigureAwait(false);
            var listOfArcKeys = productionStoreDetails.ToDictionaryList();
            foreach (var arcKey in listOfArcKeys)
            {
                await new BlobConnector(arcKey[Constants.ArchiveKeyName].ToString()).DeleteAllContainerAsync();
            }
        }

        [Then(@"Clear WIP Storage Account")]
        public async Task ThenClearWIPStorageAccount()
        {
            var productionStoreDetails = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetDistinctWIPKeyNames).ConfigureAwait(false);
            var listOfWIPKeys = productionStoreDetails.ToDictionaryList();
            foreach (var wipKey in listOfWIPKeys)
            {
                await new FileShareConnector(wipKey[Constants.WIPKeyName].ToString()).DeleteAllFileShareAsync();
            }

            // clear all Production from DB
            await this.ReadSqlAsStringDictionaryAsync(SqlQueries.DeleteAllProductions).ConfigureAwait(false);

            // delete all production stores from DB
            await this.ReadSqlAsStringDictionaryAsync(SqlQueries.DeleteAllProductionStores).ConfigureAwait(false);
        }

        [Then(@"I verify that production in ""(.*)"" is deleted from WIP")]
        public async Task ThenIVerifyThatProductionInIsDeletedFromWIP(string productionStore)
        {
            FileShareConnector fs = new FileShareConnector();
            productionStore = fs.WipStorageSettings.AllKeys.Contains(productionStore) ? fs.WipStorageSettings[productionStore] : productionStore;
            var productionStoreDetails = await this.ReadSqlAsDictionaryAsync(SqlQueries.GetProductionStoreDetails, args: new { name = productionStore }).ConfigureAwait(false);
            var productionExists = await new FileShareConnector(productionStoreDetails[Constants.WIPKeyName].ToString()).SearchDirectoryInShare(this.ScenarioContext[Constants.ProductionDirName].ToString());
            Assert.IsFalse(productionExists);
        }

        // [Then(@"Copy Test Data to WIP from Test SA")]
        // public async Task ThenMoveTestDataAsync()
        // {
        //    DataMovementConnector dmc = new DataMovementConnector();
        //    await dmc.MoveTestData();
        // }
        [Then(@"Move Test Data")]
        public async Task ThenMoveTestDataBlobToBlobAsync()
        {
            DataMovementConnector dmc = new DataMovementConnector();
            await dmc.MoveTestDataFromBlobToBlob();
        }

        [Then(@"Copy Test Data to WIP from Test SA")]
        public async Task MoveTestData()
        {
            BlobConnector bc = new BlobConnector(this.StorageAccountDetails["sa_TestData_Key"]);
            var fullContainerList = await bc.ListContainersAsync();
            for (int i = 1; i <= int.Parse(this.StorageAccountDetails["NoOfWIPArcPairs"]); i++)
            {
                var containerList = fullContainerList.Where(x => x.StartsWith($"sa{i}"));
                foreach (string containerName in containerList)
                {
                    string targetContainerName = containerName.Replace($"sa{i}-", string.Empty);
                    DataMovementConnector dmc = new DataMovementConnector(this.StorageAccountDetails["sa_TestData_Key"], this.StorageAccountDetails[$"sa{i}_WIP_Key"]);
                    var container = dmc.BlobClient.GetContainerReference(containerName);
                    var rootBlobDirectory = container.GetDirectoryReference(string.Empty);
                    Console.WriteLine(rootBlobDirectory);

                    var fileShare = dmc.FileClient.GetShareReference(targetContainerName);

                    // If Production store already exists , data will not be copied: Can be removed if needed
                    if (!await fileShare.ExistsAsync())
                    {
                        await fileShare.CreateIfNotExistsAsync();

                        // add code for db entry for PS
                        var mGroup = this.GetUserGroupNames(targetContainerName, "mg");
                        var uGroup = this.GetUserGroupNames(targetContainerName, "ug");

                        await this.ReadSqlAsStringDictionaryAsync(SqlQueries.InsertProductionStore, args: new { psId = Guid.NewGuid().ToString(), productionStoreName = targetContainerName, region = this.StorageAccountDetails[$"sa{i}_Region"], wipKey = this.StorageAccountDetails[$"sa{i}_WIP_Key"], arcKey = this.StorageAccountDetails[$"sa{i}_Arc_Key"], mGroup = mGroup, uGroup = uGroup }).ConfigureAwait(false);

                        var rootShareDir = fileShare.GetRootDirectoryReference();

                        var directoryList = await bc.ListAllLevel1FoldersInAContainer(containerName);

                        if (dmc.TestDataStorageSettings["UseLargeSizeProductionsForTesting"].ToString().EqualsIgnoreCase("No"))
                        {
                            directoryList.RemoveAll((x) => (x.ContainsIgnoreCase("Large") || x.ContainsIgnoreCase("Medium")));
                        }

                        if (directoryList.Count == 0)
                        {
                            await this.CreateProdUsingAPIAsync(targetContainerName, $"{targetContainerName}-Empty");
                        }

                        foreach (string directory in directoryList)
                        {
                            var fileShareDirectory = rootShareDir.GetDirectoryReference(directory);

                            // If Production already exists , data will not be copied : Can be removed if needed
                            if (!await fileShareDirectory.ExistsAsync())
                            {
                                // add code to use API to create production
                                await this.CreateProdUsingAPIAsync(targetContainerName, directory);
                                Assert.IsNotNull(this.Result, $"Create Production failed with Response code{this.ScenarioContext["ResponseMsg"]}");

                                Thread.Sleep(5000);
                                var blobDirectory = rootBlobDirectory.GetDirectoryReference(directory);
                                await dmc.TransferFilesAsync(fileShareDirectory, blobDirectory).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
        }
    }
}
