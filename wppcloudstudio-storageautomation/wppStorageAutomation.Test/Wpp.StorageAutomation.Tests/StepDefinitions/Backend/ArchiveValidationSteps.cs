// <copyright file="ArchiveValidationSteps.cs" company="Microsoft">
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//    OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//    OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace Wpp.StorageAutomation.Tests.StepDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Bdd.Core.Utils;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    using TechTalk.SpecFlow;

    using Wpp.StorageAutomation.Tests.Core;
    using Wpp.StorageAutomation.Tests.Entities;
    using Wpp.StorageAutomation.Tests.Executors;

    [Binding]
    public class ArchiveValidationSteps : WppApiStepDefinitionBase
    {
        [Then(@"I get the hierarchy of files in Archived storage")]
        public async Task WhenIReadBlobListAsync()
        {
            BlobConnector bc = await this.CreateArcConnectorAsync().ConfigureAwait(false);
            var blobList = await bc.ListAllBlobsInAllContainers();
            this.ScenarioContext["ArchivedListOfFilesInProd"] = blobList;
            Console.WriteLine(blobList);
        }

        [Then(@"I get the hierarchy of files in Archive ProductionStore ""(.*)""")]
        public async Task WhenIReadBlobListInProdAsync(string ps)
        {
            BlobConnector bc = await this.CreateArcConnectorAsync().ConfigureAwait(false);

            // ps = await this.GetArchivedPSGUIDAsync(ps);
            var blobList = await bc.ListAllBlobsInGivenContainer(ps);
            this.ScenarioContext["ArchivedListOfFilesInProd"] = blobList;
            Console.WriteLine(blobList);
        }

        [Then(@"I get the hierarchy of files in Archived Production ""(.*)"" under production store ""(.*)""")]
        public async Task WhenIReadBlobListInProdAsync(string prod, string ps)
        {
            BlobConnector bc = await this.CreateArcConnectorAsync().ConfigureAwait(false);

            // ps = await this.GetArchivedPSGUIDAsync(ps);
            var blobList = await bc.ListAllBlobsInGivenContainer(ps, $"{prod}/");

            // replace GUID with ProdName
            blobList = await this.ReplaceGuidWithProdName(blobList);

            this.ScenarioContext["ArchivedListOfFilesInProd"] = blobList;
            Console.WriteLine(blobList);
        }

        [Then(@"I verify that all folders and sub folders under ""(.*)"" are restored")]
        public async Task ThenIVerifyThatAllFoldersAndSubFoldersUnderAreRestoredAsync(string prod)
        {
            FileShareConnector fs = await this.CreateWIPConnectorAsync(this.ScenarioContext[Constants.WIPStore].ToString()).ConfigureAwait(false);
            var prodDirectoryName = fs.WipStorageSettings.AllKeys.Contains(prod) ? fs.WipStorageSettings[prod] : prod;
            List<string> wipListAfterRestore = await fs.ListProductionFoldersAndFiles(prodDirectoryName, this.ScenarioContext[Constants.WIPStore].ToString());
            List<string> wipListBeforeRestore = this.ScenarioContext.Get<List<string>>(Constants.WipListBeforeRestore);

            Assert.IsTrue(wipListBeforeRestore.All(wipListAfterRestore.Contains), $"Below files are not restored:\n {string.Join(",", wipListBeforeRestore.Except(wipListAfterRestore).ToArray())} \n {string.Join(",", wipListAfterRestore.Except(wipListBeforeRestore).ToArray())}");
        }

        // [Then(@"I verify that newly uploaded files under ""(.*)"" are Archived")]
        // [Then(@"I verify that all folders and sub folders under ""(.*)"" are archived")]
        // public async Task ThenIVerifyThatAllFoldersAndSubFoldersUnderAreArchivedAsync(string prod)
        // {
        //    FileShareConnector fs = await this.CreateWIPConnectorAsync()ConfigureAwait(false);;
        //    List<string> wipList = await fs.ListFileSharesAsync();
        //    await this.WhenIReadBlobListInProdAsync(prod);
        //    List<string> archivedList = this.ScenarioContext.Get<List<string>>("ArchivedListOfFilesInProd");
        //    this.ScenarioContext["archivedList"] = archivedList;

        // // Assert.AreEqual(wipList, archivedList, $"Below files are not archived:\n {wipList.Except(archivedList).ToList()}");
        //    Assert.IsTrue(wipList.All(archivedList.Contains), $"Below files are not archived:\n {string.Join(",", wipList.Except(archivedList).ToArray())} \n {string.Join(",", archivedList.Except(wipList).ToArray())}");
        // }
        [Then(@"I verify that all Productions and its files under all Production stores are archived")]
        [StepDefinition(@"I verify that newly uploaded files are archived")]
        public async Task ThenIVerifyThatAllProductionsAndItsFilesUnderAllProductionStoresAreArchived()
        {
            // added this check because in few cases we are calling archive all production stores api without adding production store name into scenario context
            // if (string.IsNullOrEmpty(this.GetValue(Constants.ProductionStoreName)))
            // {
            //    this.ScenarioContext[Constants.ProductionStoreName] = "productionstore-a";
            // }
            List<string> wipList = new List<string>();
            List<string> archivedList = new List<string>();

            for (int i = 1; i <= int.Parse(this.StorageAccountDetails["NoOfWIPArcPairs"]); i++)
            {
                FileShareConnector fs = new FileShareConnector(this.StorageAccountDetails[$"sa{i}_WIP_Key"]);
                BlobConnector bc = new BlobConnector(this.StorageAccountDetails[$"sa{i}_Arc_Key"]);
                wipList.AddRange(await fs.ListAllShareHierarchy());
                archivedList.AddRange(await bc.ListAllBlobsInAllContainers());
            }

            this.RemoveMetadataFileFromList(ref archivedList);
            archivedList = await this.ReplaceGuidWithProdName(archivedList);

            this.ScenarioContext["archivedList"] = archivedList;

            // Assert.AreEqual(wipList, archivedList, $"Below files are not archived:\n {wipList.Except(archivedList).ToList()}");
            Assert.IsTrue(wipList.All(archivedList.Contains), $"Below files are not archived/restored:\n {string.Join(",", wipList.Except(archivedList).ToArray())} \n {string.Join(",", archivedList.Except(wipList).ToArray())}");
        }

        [Then(@"the archive is created in the correct production store archive storage account")]
        public async Task ThenTheArchiveIsCreatedInTheCorrectProductionStoreArchiveStorageAccount()
        {
            await this.VerifyArchiveUsingWIPKey(this.ScenarioContext[Constants.MultipleSA1].ToString()).ConfigureAwait(false);
            await this.VerifyArchiveUsingWIPKey(this.ScenarioContext[Constants.MultipleSA2].ToString()).ConfigureAwait(false);
        }

        public async Task VerifyArchiveUsingWIPKey(string productionStoreName)
        {
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetProductionStoreDetails, args: new { name = productionStoreName }).ConfigureAwait(false);
            FileShareConnector fs = new FileShareConnector(productionDetails[Constants.WIPKeyName]);
            List<string> wipList = await fs.ListShareHierarchy(productionStoreName);

            BlobConnector bc = new BlobConnector(productionDetails[Constants.ArchiveKeyName]);

            // string psGUID = await this.GetArchivedPSGUIDAsync(productionStoreName);
            List<string> archivedList = await bc.ListAllBlobsInGivenContainer(productionStoreName);
            this.RemoveMetadataFileFromList(ref archivedList);
            archivedList = await this.ReplaceGuidWithProdName(archivedList);

            // Assert.AreEqual(wipList, archivedList, $"Below files are not archived:\n {wipList.Except(archivedList).ToList()}");
            Assert.IsTrue(wipList.All(archivedList.Contains), $"Below files are not archived:\n {string.Join(",", wipList.Except(archivedList).ToArray())} \n {string.Join(",", archivedList.Except(wipList).ToArray())}");
        }

        [StepDefinition(@"I verify that newly uploaded files under ""(.*)"" Production store are Archived")]
        [StepDefinition(@"I verify that all folders and sub folders under ""(.*)"" Production store are archived")]
        public async Task ThenIVerifyThatAllFoldersAndSubFoldersUnderPSAreArchivedAsync(string ps)
        {
            string productionStoreName = this.WipStorageSettings[ps];
            FileShareConnector fs = await this.CreateWIPConnectorAsync(productionStoreName).ConfigureAwait(false);
            List<string> wipList = await fs.ListShareHierarchy(productionStoreName);

            BlobConnector bc = await this.CreateArcConnectorAsync(productionStoreName).ConfigureAwait(false);
            List<string> archivedList = await bc.ListAllBlobsInGivenContainer(productionStoreName);
            this.RemoveMetadataFileFromList(ref archivedList);
            archivedList = await this.ReplaceGuidWithProdName(archivedList);

            // Assert.AreEqual(wipList, archivedList, $"Below files are not archived:\n {wipList.Except(archivedList).ToList()}");
            Assert.IsTrue(wipList.All(archivedList.Contains), $"Below files are not archived:\n {string.Join(",", wipList.Except(archivedList).ToArray())} \n {string.Join(",", archivedList.Except(wipList).ToArray())}");
        }

        [Then(@"""(.*)"" file ""(.*)"" path under ""(.*)"" should not be restored")]
        public async Task ThenFilePathUnderShouldBeRestored(string file, string productionName, string ps)
        {
            string productionStoreName = this.WipStorageSettings[ps];
            FileShareConnector fs = await this.CreateWIPConnectorAsync(productionStoreName).ConfigureAwait(false);
            List<string> wipList = await fs.ListShareHierarchy(productionStoreName);
            Assert.IsFalse(wipList.Contains(file));
        }

        [Then(@"I verify that ""(.*)"" is Archived and a hidden Json file is present in the Archived production")]
        public async Task ThenIVerifyThatIsArchivedAndAHiddenJsonFileIsPresentInTheArchivedProduction(string prod)
        {
            string psName = this.ArchiveStorageSettings[prod.Split("_")[0]];
            prod = this.ArchiveStorageSettings.AllKeys.Contains(prod) ? this.ArchiveStorageSettings[prod] : prod;
            BlobConnector bc = await this.CreateArcConnectorAsync(psName).ConfigureAwait(false);
            string prodGUID = await this.GetArchivedProdGUIDAsync(prod);
            List<string> archivedList = await bc.ListAllBlobsInAllContainers();
            archivedList = await this.ReplaceGuidWithProdName(archivedList);
            Assert.IsTrue(archivedList.Contains($"{prod}/{prodGUID}.metadata"), $"{prod} Not archived successfully with meatadata json file");
        }

        [Then(@"I verify that all files of below formats under ""(.*)"" are Archived")]
        public async Task ThenIVerifyThatAllFilesOfBelowFormatsUnderAreArchived(string ps, Table fileFormats)
        {
            string productionStoreName = this.ArchiveStorageSettings[ps];
            BlobConnector bc = await this.CreateArcConnectorAsync(productionStoreName).ConfigureAwait(false);

            // string psGUID = await this.GetArchivedPSGUIDAsync(productionStoreName);
            List<string> archivedList = await bc.ListAllBlobsInGivenContainer(productionStoreName);
            archivedList = await this.ReplaceGuidWithProdName(archivedList);
            this.CheckIfFileExist(archivedList, fileFormats);
        }

        [Then(@"I verify that all files of below formats are Archived")]
        public async Task ThenIVerifyThatAllFilesOfBelowFormatsAreArchived(Table fileFormats)
        {
            string productionStoreName = this.ArchiveStorageSettings["ProductionStore1"];
            BlobConnector bc = await this.CreateArcConnectorAsync(productionStoreName).ConfigureAwait(false);
            List<string> archivedList = await bc.ListAllBlobsInAllContainers();
            archivedList = await this.ReplaceGuidWithProdName(archivedList);
            this.CheckIfFileExist(archivedList, fileFormats);
        }

        [Then(@"I verify the content of ""(.*)"" file in ""(.*)"" path in Archive storage")]
        public async Task ThenIVerifyTheContentOfFileInPathInArchiveStorageAsync(string file, string path)
        {
            string psName = this.ArchiveStorageSettings[path.Split("_")[0]];
            BlobConnector bc = await this.CreateArcConnectorAsync(psName).ConfigureAwait(false);

            string prodGuid = await this.GetArchivedProdGUIDAsync(bc.ArchiveStorageSettings[path]);
            string blobName = $"{prodGuid}/{file}";
            string blobFileContent = await bc.ReadBlobAsync(this.ScenarioContext["ProductionStoreName"].ToString(), blobName);

            Assert.AreEqual(this.ScenarioContext["FileContent"].ToString(), blobFileContent, "Latest version is not archived");
        }

        [Then(@"I verify ""(.*)"" folder in ""(.*)"" is deleted from Archive container")]
        [Then(@"I verify ""(.*)"" file in ""(.*)"" is deleted from Archive container")]
        public async Task ThenIVerifyFileInIsDeletedFromArchiveContainer(string file, string path)
        {
            string psName = this.ArchiveStorageSettings[path.Split("_")[0]];
            BlobConnector bc = await this.CreateArcConnectorAsync(psName).ConfigureAwait(false);

            string prodGuid = await this.GetArchivedProdGUIDAsync(bc.ArchiveStorageSettings[path]);
            string blobName = $"{prodGuid}/{file}";
            string blobFileContent = await bc.ReadBlobAsync(this.ScenarioContext["ProductionStoreName"].ToString(), blobName);
            Assert.IsTrue(string.IsNullOrEmpty(blobFileContent));
        }

        [Then(@"I verify that all folders and sub folders under a production are restored successfully from UI")]
        public async Task ThenIVerifyThatAllFoldersAndSubFoldersUnderAreRestoredSuccessfullyFromUI()
        {
            await this.ThenIVerifyThatAllFoldersAndSubFoldersUnderAreRestoredAsync(this.ScenarioContext[Constants.ProductionDirName].ToString()).ConfigureAwait(false);
        }

        [Then(@"Verify all productions in the ""(.*)"" Production Store have respective metadata file")]
        public async Task ThenVerifyAllProductionsInTheProductionStoreHaveRespectiveMetadataFile(string productionStore)
        {
            var containerName = this.ArchiveStorageSettings.AllKeys.Contains(productionStore) ? this.ArchiveStorageSettings[productionStore] : productionStore;
            BlobConnector bc = await this.CreateArcConnectorAsync(containerName).ConfigureAwait(false);
            List<string> productionsList = await bc.ListAllLevel1FoldersInAContainer(containerName);
            foreach (string prodNameInArchive in productionsList)
            {
                string fileName = $"{prodNameInArchive}/{prodNameInArchive}" + ".metadata";
                string blobFileContent = await bc.ReadBlobAsync(containerName, fileName);
                Assert.IsNotEmpty(blobFileContent.ToString(), $"{fileName} not present in {prodNameInArchive} production");
            }
        }

        [Then(@"Verify all productions in the all Production Stores have respective metadata file")]
        public async Task ThenVerifyAllProductionsInTheAllProductionStoresHaveRespectiveMetadataFile()
        {
            var productionStoreDetails = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetDistinctArchiveKeyNames).ConfigureAwait(false);
            var listOfArcKeys = productionStoreDetails.ToDictionaryList();
            foreach (var arcKey in listOfArcKeys)
            {
                BlobConnector bc = new BlobConnector(arcKey[Constants.ArchiveKeyName].ToString());
                List<string> containersList = await bc.ListContainersAsync();
                foreach (string containerName in containersList)
                {
                    List<string> productionsListInContainer = await bc.ListAllLevel1FoldersInAContainer(containerName);
                    foreach (string prodNameInArchive in productionsListInContainer)
                    {
                        string fileName = $"{prodNameInArchive}/{prodNameInArchive}" + ".metadata";
                        string blobFileContent = await bc.ReadBlobAsync(containerName, fileName);
                        Assert.IsNotEmpty(blobFileContent.ToString(), $"{fileName} not present in {prodNameInArchive} production in {containerName} store");
                    }
                }
            }
        }

        [StepDefinition(@"Verify ""(.*)"" Production contains the metadata file")]
        public async Task ThenVerifyProductionInProductionStoreContainsTheMetadataFile(string production)
        {
            var containerName = this.ScenarioContext["ProductionStoreName"].ToString();
            BlobConnector bc = await this.CreateArcConnectorAsync(containerName).ConfigureAwait(false);
            var prodName = this.WipStorageSettings.AllKeys.Contains(production) ? this.WipStorageSettings[production] : production;
            this.ScenarioContext[Constants.ProductionName] = prodName;
            string prodGUID = await this.GetArchivedProdGUIDAsync(prodName);
            string fileName = $"{prodGUID}/{prodGUID}" + ".metadata";
            string blobFileContent = await bc.ReadBlobAsync(containerName, fileName);
            this.ScenarioContext[Constants.FileContent] = blobFileContent;
            Assert.IsNotEmpty(blobFileContent.ToString(), $"{fileName} not present in {prodName} production in {containerName} store");
        }

        [StepDefinition(@"Verify contents of metadata file")]
        public async Task ThenVerifyContentsOfFile()
        {
            FileShareConnector fs = await this.CreateWIPConnectorAsync().ConfigureAwait(false);
            List<string> wipList = await fs.ListProductionFoldersAndFiles(this.ScenarioContext[Constants.ProductionName].ToString(), this.ScenarioContext["ProductionStoreName"].ToString());
            var metadata = JObject.Parse(this.ScenarioContext[Constants.FileContent].ToString()).SelectToken("Items");
            foreach (var item in wipList)
            {
                Assert.NotNull(metadata[item.Replace(" ", "%20")]["Name"], $"Name empty for {item}");
                Assert.NotNull(metadata[item.Replace(" ", "%20")]["Type"], $"Type empty for {item}");
                Assert.NotNull(metadata[item.Replace(" ", "%20")]["LastWriteTimeUtc"], $"LastWriteTimeUtc empty for {item}");
                Assert.NotNull(metadata[item.Replace(" ", "%20")]["CreationTimeUtc"], $"CreationTimeUtc empty for {item}");
                Assert.NotNull(metadata[item.Replace(" ", "%20")]["AbsolutePath"], $"AbsolutePath empty for {item}");
            }
        }

        [Then(@"I verify ""(.*)"" file and ""(.*)"" production has updated created time")]
        public void ThenIVerifyFileAndProductionHasUpdatedCreatedTime(string fileName, string prodName)
        {
            var settings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.DateTimeOffset,
            };
            FileShareConnector fs = new FileShareConnector();
            var prod = fs.WipStorageSettings.AllKeys.Contains(prodName) ? fs.WipStorageSettings[prodName] : prodName;
            var metadata = JsonConvert.DeserializeObject<JObject>(this.ScenarioContext[Constants.FileContent].ToString(), settings).SelectToken("Items");
            var prodPath = "/" + this.ScenarioContext["ProductionStoreName"].ToString() + "/" + prod;
            Assert.IsTrue(metadata[prodPath]["CreationTimeUtc"].ToString().CompareTo(this.ScenarioContext[Constants.CreatedDateTime] + " +00:00") <= 10, $"CreationTimeUtc wrong for {prod}");
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(metadata[prodPath + "/" + fileName]["CreationTimeUtc"].ToString()), Convert.ToDateTime(this.ScenarioContext[Constants.FileUploadTimeStamp])) <= 0, $"CreationTimeUtc wrong for {fileName}");
            Assert.IsTrue(DateTime.Compare(Convert.ToDateTime(metadata[prodPath + "/" + fileName]["LastWriteTimeUtc"].ToString()), Convert.ToDateTime(this.ScenarioContext[Constants.FileUploadTimeStamp])) <= 0, $"LastWriteTimeUtc wrong for {fileName}");
        }

        [Then(@"I wait till ""(.*)"" min from last archive time and additional ""(.*)"" min for the archive to complete")]
        public void ThenIWaitTillMinFromLastArchiveTimeAndAdditionalMinForTheArchiveToComplete(int archiveInterval, int waitTime)
        {
            int timetoArchive = (int)(this.ScenarioContext["LastSyncDateTime"].ToDateTime().AddMinutes(archiveInterval) - DateTime.UtcNow).TotalMilliseconds;
            Assert.IsTrue(timetoArchive > 0, $"Last Archive did not happen in 15 min: Last Archive Time- {this.ScenarioContext["LastSyncDateTime"]} , Current time - {DateTime.UtcNow}");

            Thread.Sleep(timetoArchive);
            Thread.Sleep(waitTime * 60 * 1000);
        }

        // =================== common methods==================================
        public void CheckIfFileExist(List<string> list, Table filesTable)
        {
            List<string> missingFormats = new List<string>();
            List<string> files = filesTable.Rows.Select(r => r["format"].ToString()).ToList();
            foreach (var file in files)
            {
                if (list.Any(x => x.EndsWith(file)))
                {
                    continue;
                }

                missingFormats.Add(file);
            }

            Assert.AreEqual(0, missingFormats.Count, $"files of these format are not Archived: {string.Join(", ", missingFormats.ToArray())}");
        }

        // public void RemoveEmptyProductionFromList(ref List<string> wipList, ref List<string> archivedList)
        // {
        //    // List<string> emptyProdList = archivedList.Where(x => x.EndsWith("empty.json")).ToList();
        //    // if (emptyProdList.Count > 0)
        //    // {
        //    //    foreach (var emptyProd in emptyProdList)
        //    //    {
        //    //        wipList.RemoveAll((x) => x.StartsWith($"{emptyProd.Split("/")[0]}/"));
        //    //    }
        //    //    archivedList.RemoveAll((x) => x.EndsWith("empty.json"));
        //    // }
        //    archivedList.RemoveAll((x) => x.EndsWith(".metadata"));

        // Console.WriteLine(wipList);
        // }
        public async Task<string> GetArchivedPSGUIDAsync(string ps)
        {
            return await this.ReadSqlScalarAsync<string>(SqlQueries.GetGuidOfPS, args: new { name = ps }).ConfigureAwait(false);
        }

        public async Task<string> GetArchivedProdGUIDAsync(string prod)
        {
            return await this.ReadSqlScalarAsync<string>(SqlQueries.GetGuidOfProd, args: new { name = prod }).ConfigureAwait(false);
        }

        public async Task<string> GetArchivedProdNameAsync(string prod)
        {
            return await this.ReadSqlScalarAsync<string>(SqlQueries.GetNameOfProdUsingId, args: new { id = prod }).ConfigureAwait(false);
        }

        public async Task<List<string>> ReplaceGuidWithProdName(List<string> archivedList)
        {
            // get lst if unique prods
            // get ProdName Dict
            // replace
            var listOfProd = archivedList.Select(x => x.Split('/')[0]).Distinct().ToList();
            Dictionary<string, string> prodMapping = new Dictionary<string, string>();

            foreach (string prod in listOfProd)
            {
                prodMapping[prod] = await this.GetArchivedProdNameAsync(prod);
            }

            // Some productions in archive do not exist in DB or WIP hence the conditional check
            archivedList = archivedList.Select(x => x.Remove(0, 36).Insert(0, prodMapping[x.Split('/')[0]] != null ? prodMapping[x.Split('/')[0]] : "NotInDB")).ToList();
            Console.WriteLine(archivedList);

            return archivedList;
        }

        public void RemoveMetadataFileFromList(ref List<string> archivedList)
        {
            archivedList.RemoveAll((x) => x.EndsWith(".metadata"));
        }
    }
}
