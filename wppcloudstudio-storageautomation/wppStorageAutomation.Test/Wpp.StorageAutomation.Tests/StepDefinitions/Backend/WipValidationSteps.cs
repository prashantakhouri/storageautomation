// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WipValidationSteps.cs" company="Microsoft">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Bdd.Core;
    using Bdd.Core.Utils;

    using global::Bdd.Core.Web.StepDefinitions;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    using Wpp.StorageAutomation.Tests.Core;
    using Wpp.StorageAutomation.Tests.Entities;
    using Wpp.StorageAutomation.Tests.Executors;

    [Binding]
    public class WipValidationSteps : WppApiStepDefinitionBase
    {
        private readonly int millisecondsDelay = ConfigManager.AppSettings["ExplicitWait"].ToInt();

        [Then(@"I get the hierarchy of files in WIP Production ""(.*)""")]
        public async Task ThenIGetTheHierarchyOfFilesInWipProduction(string productionName)
        {
            this.ScenarioContext[Constants.ProductionDirName] = productionName;
            FileShareConnector fs = await this.CreateWIPConnectorAsync().ConfigureAwait(false);
            var list = await fs.ListProductionHierarchy("Default", productionName);
            this.ScenarioContext["WIPListOfFilesInProd"] = list;
            foreach (string i in list)
            {
                Console.WriteLine(i.ToString());
            }
        }

        [StepDefinition(@"production ""(.*)"" is created in ""(.*)"" production store in ""(.*)"" seconds")]
        [StepDefinition(@"Verify Production ""(.*)"" is restored in ""(.*)"" production store in ""(.*)"" seconds")]
        [StepDefinition(@"Verify Production ""(.*)"" is created in ""(.*)"" production store in ""(.*)"" seconds")]
        public async Task ThenVerifyProductionIsCreated(string productionName, string productionStore, float delay)
        {
            // FileShareConnector fs = new FileShareConnector();
            var prodName = this.WipStorageSettings.AllKeys.Contains(productionName) ? this.WipStorageSettings[productionName] : productionName;
            this.ScenarioContext[Constants.ProductionDirName] = prodName;
            productionStore = this.WipStorageSettings.AllKeys.Contains(productionStore) ? this.WipStorageSettings[productionStore] : productionStore;
            this.ScenarioContext[Constants.ProductionStoreName] = productionStore;
            var waitInterval = ((int)delay * 1000) / this.millisecondsDelay;
            FileShareConnector fileShare = await this.CreateWIPConnectorAsync().ConfigureAwait(false);
            do
            {
                await Task.Delay(this.millisecondsDelay).ConfigureAwait(true);
                bool exists = await fileShare.SearchDirectoryInShare(prodName, productionStore);
                if (exists)
                {
                    break;
                }

                waitInterval--;
            }
            while (waitInterval > 0);

            bool isExists = await fileShare.SearchDirectoryInShare(prodName, productionStore);
            Assert.IsTrue(isExists, $"Production Does not exist :{prodName} in WIP:{productionStore}");
        }

        [Then(@"Verify Production ""(.*)"" is deleted from ""(.*)"" production store in WIP in ""(.*)"" seconds")]
        public async Task ThenVerifyProductionIsDeletedFromProductionStoreInWIPInSeconds(string productionName, string productionStore, int delay)
        {
            var prodName = this.WipStorageSettings.AllKeys.Contains(productionName) ? this.WipStorageSettings[productionName] : productionName;
            this.ScenarioContext[Constants.ProductionDirName] = prodName;
            productionStore = this.WipStorageSettings.AllKeys.Contains(productionStore) ? this.WipStorageSettings[productionStore] : productionStore;
            this.ScenarioContext[Constants.ProductionStoreName] = productionStore;
            await Task.Delay(delay).ConfigureAwait(true);
            FileShareConnector fileShare = await this.CreateWIPConnectorAsync().ConfigureAwait(false);
            bool isExists = await fileShare.SearchDirectoryInShare(prodName, productionStore);
            Assert.IsFalse(isExists, $"Production {prodName}  is still present in Production store:{productionStore}");
        }

        // [Then(@"Verify FS")]
        // public async Task ThenVerifyFSAsync()
        // {
        //    FileShareConnector fs = await this.CreateWIPConnectorAsync().ConfigureAwait(false);
        //    this.ScenarioContext["FS"] = fs;
        //    await fs.ListFSAsync();
        // }
        [Then(@"Verify Production ""(.*)"" is not created in ""(.*)"" production store")]
        public async Task ThenVerifyProductionIsNotCreated(string productionName, string productionStore)
        {
            FileShareConnector fileShare = new FileShareConnector();
            Thread.Sleep(2000);
            var prodName = fileShare.WipStorageSettings.AllKeys.Contains(productionName) ? fileShare.WipStorageSettings[productionName] : productionName;
            this.ScenarioContext[Constants.ProductionDirName] = prodName;
            var shareName = fileShare.WipStorageSettings.AllKeys.Contains(productionStore) ? fileShare.WipStorageSettings[productionStore] : productionStore;
            this.ScenarioContext[Constants.ProductionStoreName] = shareName;
            FileShareConnector fs = await this.CreateWIPConnectorAsync().ConfigureAwait(false);
            bool isExists = await fs.SearchDirectoryInShare(prodName, shareName);
            Assert.IsFalse(isExists, $"Production should not exist :{prodName} in share:{shareName}");
        }

        [Then(@"Verify Share")]
        public async Task ThenVerifyShare()
        {
            FileShareConnector fs = await this.CreateWIPConnectorAsync().ConfigureAwait(false);
            var list = await fs.ListShareHierarchy();
            foreach (string i in list)
            {
                Console.WriteLine(i.ToString());
            }
        }

        ////[Then(@"Delete Production ""(.*)""")]
        ////public async Task ThenDeleteProduction(string productionName)
        ////{
        ////    FileShareConnector fs = await this.CreateWIPConnectorAsync().ConfigureAwait(false);
        ////    await fs.DeleteProduction("Default", productionName);
        ////}

        [When(@"I upload ""(.*)"" file in ""(.*)"" path in WIP storage")]
        public async Task WhenIUploadFileInPathInWIPStorageAsync(string file, string directory)
        {
            FileShareConnector fs = await this.CreateWIPConnectorAsync().ConfigureAwait(false);
            string productionStore = this.ScenarioContext["ProductionStoreName"].ToString();
            directory = fs.WipStorageSettings[directory];
            await fs.UploadFile(productionStore, directory, file);
        }

        [StepDefinition(@"I upload ""(.*)"" file in ""(.*)"" path under ""(.*)"" in WIP storage")]
        public async Task WhenIUploadFileInPathUnderInWIPStorage(string file, string directory, string ps)
        {
            string productionStore = this.WipStorageSettings[ps];
            FileShareConnector fs = await this.CreateWIPConnectorAsync(productionStore).ConfigureAwait(false);
            this.ScenarioContext["ProductionStoreName"] = productionStore;
            directory = this.WipStorageSettings[directory];
            await fs.UploadFile(productionStore, directory, file);
            this.ScenarioContext[Constants.FileUploadTimeStamp] = DateTime.Now;
        }

        [When(@"I edit the content in ""(.*)"" file in ""(.*)"" path in WIP storage")]
        public async Task WhenIEditTheContentInFileInPathInWIPStorageAsync(string file, string directory)
        {
            string psName = this.ArchiveStorageSettings[directory.Split("_")[0]];
            FileShareConnector fs = await this.CreateWIPConnectorAsync(psName).ConfigureAwait(false);
            string productionStore = this.ScenarioContext["ProductionStoreName"].ToString();
            directory = fs.WipStorageSettings[directory];
            string fileContent = await fs.EditFile(productionStore, directory, file);
            this.ScenarioContext["FileContent"] = fileContent;
        }

        [When(@"I have deleted ""(.*)"" folder in ""(.*)"" path in WIP storage")]
        [When(@"I have deleted ""(.*)"" file in ""(.*)"" path in WIP storage")]
        public async Task WhenIHaveDeletedFileInPathInWIPStorage(string file, string directory)
        {
            string psName = this.ArchiveStorageSettings[directory.Split("_")[0]];
            FileShareConnector fs = await this.CreateWIPConnectorAsync(psName).ConfigureAwait(false);
            string productionStore = this.ScenarioContext["ProductionStoreName"].ToString();
            directory = fs.WipStorageSettings[directory];
            if (file.ContainsIgnoreCase("DummyTestFile"))
            {
                await fs.DeleteFileInFileShareAsync(productionStore, directory, file).ConfigureAwait(false);
            }
            else
            {
                await fs.DeleteFolderInFileShareAsync(productionStore, directory, file).ConfigureAwait(false);
            }
        }

        [When(@"I upload ""(.*)"" folder in ""(.*)"" path under ""(.*)"" in WIP storage")]
        public void WhenIUploadFolderInPathUnderInWIPStorage(string p0, string p1, string p2)
        {
            //// already taken care
        }

        /*Removing now, since make offline is implemented, will delete once it has no impact on pipeline
        [Given(@"I delete ""(.*)"" Production from ""(.*)"" Production Store")]
        public async Task GivenIDeleteProductionFromProductionStore(string productionName, string productionStore)
        {
            var prodName = this.WipStorageSettings.AllKeys.Contains(productionName) ? this.WipStorageSettings[productionName] : productionName;
            this.ScenarioContext[Constants.ProductionDirName] = prodName;
            productionStore = this.WipStorageSettings.AllKeys.Contains(productionStore) ? this.WipStorageSettings[productionStore] : productionStore;
            this.ScenarioContext[Constants.ProductionStoreName] = productionStore;
            FileShareConnector fs = await this.CreateWIPConnectorAsync(productionStore).ConfigureAwait(false);
            this.ScenarioContext[Constants.WipListBeforeRestore] = await fs.ListProductionFoldersAndFiles(prodName, productionStore.ToString());
            await fs.DeleteProduction(prodName, productionStore);
            string psGUID = await this.GetPSGUIDAsync(productionStore);
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetailsInProductionStore, args: new { name = prodName, productionStoreId = psGUID }).ConfigureAwait(false);
            this.ScenarioContext[Constants.CreatedDateTime] = productionDetails[Constants.CreatedDateTime];
            await this.ReadSqlAsync(SqlQueries.UpdateProductionStatusById, args: new { status = "Offline", id = productionDetails[Constants.Id] }).ConfigureAwait(false);

            // Clear Production from DB
            // await this.ReadSqlAsStringDictionaryAsync(SqlQueries.DeleteProductionFromProductionStore, args: new { name = prodName, productionStoreId = shareName }).ConfigureAwait(false);
        }*/
    }
}