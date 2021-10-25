// <copyright file="CommonApiSteps.cs" company="Microsoft">
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

    using Bdd.Core;
    using Bdd.Core.Entities;
    using Bdd.Core.Utils;

    using FluentAssertions;

    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    using Wpp.StorageAutomation.Tests.Core;
    using Wpp.StorageAutomation.Tests.Entities;

    using Wpp.StorageAutomation.Tests.Executors;

    [Binding]
    public class CommonApiSteps : WppApiStepDefinitionBase
    {
        private readonly int millisecondsDelay = ConfigManager.AppSettings["ExplicitWait"].ToInt();

        [Given(@"I am authenticated as ""(.*)""")]
        [Given(@"I am authenticated as ""(.*)"" user")]
        public void GivenIAmAuthenticatedAsUser(string user)
        {
            this.ScenarioContext[Constants.User] = user;
        }

        [Given(@"that I have access to a particular production store")]
        [Given(@"that I provide the connection string key vault key name in the register production store endpoint request body")]
        public void GivenThatIProvideTheConnectionStringKeyVaultKeyNameInTheRegisterProductionStoreEndpointRequestBody()
        {
            // already passing keyvault names in the production store request body
        }

        [Given(@"I delete existing ""(.*)"" productionstore from database")]
        public async Task GivenIDeleteExistingProductionstoreFromDatabase(string productionStore)
        {
            var name = this.WipStorageSettings.AllKeys.Contains(productionStore) ? this.WipStorageSettings[productionStore] : productionStore;
            await this.ReadSqlAsStringDictionaryAsync(SqlQueries.DeleteAProductionStore, args: new { name }).ConfigureAwait(false);
        }

        [When(@"I send the ""(.*)"" POST request with duplicate production store")]
        [When(@"I send the ""(.*)"" POST request with production store which is not exist in the WIP and ARC storage accounts")]
        [StepDefinition(@"I send the ""(.*)"" API with below data")]
        public async Task WhenISendTheAPIWithBelowData(string api, Table table)
        {
            var url = this.Endpoint + GenericExtensions.GetResourceValue(api.ToPascalCase(), "APIRoutes");
            this.CreateJsonRequestBody(api.ToPascalCase(), table);
            if (this.ScenarioContext[Constants.User].ToString().EqualsIgnoreCase(Constants.Unauthorized))
            {
                this.RequestHeaders.Remove(Constants.AppAuthToken);
            }

            this.Result = await this.PostCallAsync(url, this.RequestObject, this.RequestHeaders);
            this.ScenarioContext[Constants.Result] = this.Result;
        }

        [When(@"I send the ""(.*)"" API with ""(.*)"" with value ""(.*)"" for ""(.*)"" Productionstore")]
        public async Task WhenISendTheAPIWithWithValue(string api, string key, string value, string productionStore)
        {
            FileShareConnector fs = new FileShareConnector();
            var ps = fs.WipStorageSettings.AllKeys.Contains(productionStore) ? fs.WipStorageSettings[productionStore] : productionStore;
            string psGUID = await this.GetPSGUIDAsync(ps);
            string url = this.Endpoint + string.Format(GenericExtensions.GetResourceValue(api.ToPascalCase(), "APIRoutes"), psGUID);
            this.CreateJsonRequestBody(api.ToPascalCase(), key, value);
            this.Result = await this.PostCallAsync(url, this.RequestObject, this.RequestHeaders);
            this.ScenarioContext[Constants.Result] = this.Result;
        }

        [When(@"I send the ""(.*)"" POST request")]
        public async Task WhenISendThePOSTRequest(string api)
        {
            var url = this.Endpoint + GenericExtensions.GetResourceValue(api.ToPascalCase(), "APIRoutes");
            this.Result = await this.PostCallAsync(url, null, this.RequestHeaders);
            this.ScenarioContext[Constants.Result] = this.Result;
        }

        [Given(@"that there are multiple production stores in different storage accounts")]
        public void GivenThatThereAreMultipleProductionStoresInDifferentStorageAccounts()
        {
            // already taken care
        }

        [Given(@"created productions in ""(.*)"" and ""(.*)"" which are in different storage accounts")]
        public async Task GivenCreatedProductionsInAndWhichAreInDifferentStorageAccounts(string ps1, string ps2)
        {
            FileShareConnector fs = new FileShareConnector();
            var productionStore = fs.WipStorageSettings.AllKeys.Contains(ps1) ? fs.WipStorageSettings[ps1] : ps1;
            this.ScenarioContext[Constants.MultipleSA1] = productionStore;
            await this.CreateProdUsingAPIAsync(productionStore, "productionstore-b-production");
            productionStore = fs.WipStorageSettings.AllKeys.Contains(ps2) ? fs.WipStorageSettings[ps2] : ps2;
            this.ScenarioContext[Constants.MultipleSA2] = productionStore;
            await this.CreateProdUsingAPIAsync(productionStore, "productionstore-d-production");
        }

        [StepDefinition(@"I send the ""(.*)"" API with below data for ""(.*)"" Productionstore")]
        public async Task WhenISendTheAPIWithBelowDataForProductionstore(string api, string productionStore, Table table)
        {
            FileShareConnector fs = new FileShareConnector();
            var ps = fs.WipStorageSettings.AllKeys.Contains(productionStore) ? fs.WipStorageSettings[productionStore] : productionStore;
            string psGUID = await this.GetPSGUIDAsync(ps);
            this.ScenarioContext[Constants.ProductionStoreName] = ps;
            string url = this.Endpoint + string.Format(GenericExtensions.GetResourceValue(api.ToPascalCase(), "APIRoutes"), psGUID);
            this.CreateJsonRequestBody(api.ToPascalCase(), table);
            this.Result = await this.PostCallAsync(url, this.RequestObject, this.RequestHeaders);
            this.ScenarioContext[Constants.Result] = this.Result;
        }

        [StepDefinition(@"I send the ""(.*)"" POST request for ""(.*)"" Productionstore")]
        public async Task WhenISendThePOSTRequestForProductionstore(string api, string psName)
        {
            FileShareConnector fs = new FileShareConnector();
            var ps = fs.WipStorageSettings.AllKeys.Contains(psName) ? fs.WipStorageSettings[psName] : psName;
            this.ScenarioContext["ProductionStoreName"] = ps;
            string psGUID = ps.ContainsIgnoreCase("Invalid") ? ps : await this.GetPSGUIDAsync(ps);
            var url = this.Endpoint + string.Format(GenericExtensions.GetResourceValue(api.ToPascalCase(), "APIRoutes"), psGUID);
            this.Result = await this.PostCallAsync(url, null, this.RequestHeaders);
            this.ScenarioContext[Constants.Result] = this.Result;
        }

        [BeforeScenario(@"ArchiveSingleProductionStore")]
        public async Task ArchiveSingleProductionAsync()
        {
            this.ScenarioContext["ProductionStoreName"] = "testproductionstorea";
            string psGUID = await this.GetPSGUIDAsync(this.ScenarioContext["ProductionStoreName"].ToString());
            var url = this.Endpoint + string.Format(GenericExtensions.GetResourceValue("ArchiveAProductionstore", "APIRoutes"), psGUID);
            this.Result = await this.PostCallAsync(url, null, this.RequestHeaders);
            this.ScenarioContext[Constants.Result] = this.Result;
            await this.WhenIVerifyIfTheAPIExecutionIsCompleteInTime("Completed", 120).ConfigureAwait(false);
        }

        [When(@"I send the ""(.*)"" GET request")]
        public async Task WhenISendTheGETRequest(string api)
        {
            var url = this.Endpoint + GenericExtensions.GetResourceValue(api.ToPascalCase(), "APIRoutes");
            this.Result = await this.GetCallAsync(url, this.RequestHeaders);
            this.ScenarioContext[Constants.Result] = this.Result;
        }

        [When(@"I send the ""(.*)"" GET request for ""(.*)"" Productionstore")]
        public async Task WhenISendTheGETRequestForProductionstore(string api, string productionStore)
        {
            FileShareConnector fs = new FileShareConnector();
            var ps = fs.WipStorageSettings.AllKeys.Contains(productionStore) ? fs.WipStorageSettings[productionStore] : productionStore;
            this.ScenarioContext["ProductionStoreName"] = ps;
            string psGUID = ps.Contains("Wrong") ? ps : await this.GetPSGUIDAsync(ps);
            var url = this.Endpoint + string.Format(GenericExtensions.GetResourceValue(api.ToPascalCase(), "APIRoutes"), psGUID);
            this.Result = await this.GetCallAsync(url, this.RequestHeaders);
            this.ScenarioContext[Constants.Result] = this.Result;
        }

        [StepDefinition(@"I verify if the API execution is ""(.*)"" with in (.*) sec")]
        public async Task WhenIVerifyIfTheAPIExecutionIsCompleteInTime(string status, float delay)
        {
            string statusQueryGetUri = JObject.FromObject(this.Result)["statusQueryGetUri"].ToString();
            JObject apiStatus = await this.GetCallAsync(statusQueryGetUri, this.RequestHeaders);
            this.ScenarioContext[Constants.RequestTriggeredDateTime] = apiStatus["createdTime"];
            var waitInterval = ((int)delay * 1000) / this.millisecondsDelay;

            do
            {
                await Task.Delay(this.millisecondsDelay).ConfigureAwait(true);
                apiStatus = await this.GetCallAsync(statusQueryGetUri, this.RequestHeaders);
                if (apiStatus["runtimeStatus"].ToString().EqualsIgnoreCase(status))
                {
                    break;
                }

                waitInterval--;
            }
            while (waitInterval > 0);

            Assert.IsTrue(apiStatus["runtimeStatus"].ToString().EqualsIgnoreCase(status), $"API execution status is not {status} in {delay} sec: {statusQueryGetUri}");
            this.ScenarioContext[Constants.ApiStatus] = apiStatus;
        }

        [StepDefinition(@"I verify if the API execution is ""(.*)"" with in (.*) min")]
        public async Task WhenIVerifyIfTheAPIExecutionIsCompleteInMin(string status, float delay)
        {
            await this.WhenIVerifyIfTheAPIExecutionIsCompleteInTime(status, delay * 60).ConfigureAwait(false);
        }

        [StepDefinition(@"I receive valid HTTP response code ""(.*)""")]
        public void ThenIReceiveValidHTTPResponseCode(string responseCode)
        {
            string responseMsg = this.ScenarioContext.Get<string>("ResponseMsg");
            Assert.IsTrue(responseMsg.Contains(responseCode), $"Response message received is :{responseMsg}");
        }

        [Then(@"Verify ""(success|failed)"" response body for ""(.*)"" API")]
        public async Task ThenVerifyResponseBodyForAPI(string result, string api, Table table)
        {
            await this.GetResponseDataForAPI();
            Dictionary<string, string> responseTokenValue = this.GetResponseTokenValue(result, api, table);
            foreach (KeyValuePair<string, string> entry in responseTokenValue)
            {
                Assert.IsTrue(this.ResponseBody.SelectToken(entry.Key).ToString().Contains(entry.Value), $"Verify response body failed for :{entry.Key}: {this.ResponseBody.SelectToken(entry.Key).ToString()}/{entry.Value} ");
            }
        }

        [Then(@"Verify ""(.*)"" sync response body for ""(.*)"" API")]
        public void ThenVerifySyncResponseBodyForAPI(string result, string api, Table table)
        {
            Dictionary<string, string> responseTokenValue = this.GetResponseTokenValue(result, api, table);
            foreach (KeyValuePair<string, string> entry in responseTokenValue)
            {
                Assert.IsTrue(JObject.Parse(this.Result.ToString()).SelectToken(entry.Key).ToString().Contains(entry.Value), $"Verify response body failed for :{entry.Key}");
            }
        }

        [Then(@"Verify ""(.*)"" sync response with ""(.*)"" for ""(.*)""")]
        public void ThenVerifySyncResponseWithFor(string result, string value, string token)
        {
            string actualResponse = JObject.Parse(this.Result.ToString()).SelectToken(token).ToString();
            Assert.IsTrue(actualResponse.Contains(value), $"Verify response body failed for :{token} \n Expected:{value}\n Actual:{actualResponse}");
        }

        [Then(@"I receive a sync response for '(.*)' API")]
        public void ThenIReceiveASyncResponseForAPI(string api)
        {
            string token = string.Empty;
            if (api.ToPascalCase() == "CreateAProductionStore" || api.ToPascalCase() == "CreateProduction")
            {
                token = "data[0]";
            }

            Assert.IsNotNull(this.Result.ToString(), $"Null Response Body for {api} api");
            Assert.IsNotNull(JObject.Parse(this.Result.ToString()).SelectToken(token), $"{token} Token nout found in response body{this.Result.ToString()}");
            string actualResponseBody = JObject.Parse(this.Result.ToString()).SelectToken(token).ToString();
            string expectedResponseBody = this.GetResponseJsonTemplate(api.ToPascalCase()).ToString();
            var instanceObjExpected = JObject.Parse(expectedResponseBody);
            var instanceObjActual = JObject.Parse(actualResponseBody);
            instanceObjActual.Should().BeEquivalentTo(instanceObjExpected);
        }

        [Then(@"I receive a response for '(.*)' API")]
        public async Task IreceiveAJsonResponse(string api)
        {
            string token = string.Empty;
            await this.GetResponseDataForAPI();
            if (api.ToPascalCase() == "CreateProduction")
            {
                token = "output.Data[0]";
            }

            string actualResponseBody = this.ResponseBody.SelectToken(token).ToString();
            string expectedResponseBody = this.GetResponseJsonTemplate(api.ToPascalCase()).ToString();
            var instanceObjExpected = JObject.Parse(expectedResponseBody);
            var instanceObjActual = JObject.Parse(actualResponseBody);
            instanceObjActual.Should().BeEquivalentTo(instanceObjExpected);
            this.ScenarioContext[Constants.GetApiResult] = instanceObjActual;
        }

        [Then(@"I receive sync response for '(.*)' API")]
        public void IreceiveSyncJsonResponse(string api)
        {
            string token = "data[0]";
            string actualResponseBody = ((JObject)this.ScenarioContext[Constants.Result]).SelectToken(token).ToString();
            string expectedResponseBody = this.GetResponseJsonTemplate(api.ToPascalCase()).ToString();
            var instanceObjExpected = JObject.Parse(expectedResponseBody);
            var instanceObjActual = JObject.Parse(actualResponseBody);
            instanceObjActual.Should().BeEquivalentTo(instanceObjExpected);
            this.ScenarioContext[Constants.GetApiResult] = instanceObjActual;
        }

        [StepDefinition(@"I send ""(.*)"" API to restore ""(.*)"" Production for ""(.*)"" production store")]
        public async Task WhenISendAPIToRestoreProductionForArchiveToWIP(string api, string productionName, string productionStore)
        {
            var ps = this.ArchiveStorageSettings.AllKeys.Contains(productionStore) ? this.ArchiveStorageSettings[productionStore] : productionStore;
            string psGUID = ps.Contains("Wrong") || ps.Contains("Invalid") ? ps : await this.GetPSGUIDAsync(ps);
            var prodName = this.ArchiveStorageSettings.AllKeys.Contains(productionName) ? this.ArchiveStorageSettings[productionName] : productionName;
            string prodGUID = await this.ReadSqlScalarAsync<string>(SqlQueries.GetGuidOfProd, args: new { name = prodName }).ConfigureAwait(false);
            this.ScenarioContext[Constants.WIPStore] = ps;
            this.ScenarioContext["ProductionDirName"] = prodName;
            var url = this.Endpoint + string.Format(GenericExtensions.GetResourceValue(api.ToPascalCase(), "APIRoutes"), psGUID, prodGUID);
            this.Result = await this.PostCallAsync(url, null, this.RequestHeaders);
            this.ScenarioContext[Constants.Result] = this.Result;
        }

        [Then(@"I verify if all productions present in the WIP storage for ""(.*)"" Productionstore are listed")]
        public async Task ThenIVerifyIfAllProductionsPresentInTheWIPStorageForProductionstoreAreListed(string productionStore)
        {
            IDictionary<string, string> productionStoreDetails = null;
            string actualResponseBody = ((JObject)this.ScenarioContext[Constants.Result]).SelectToken("data[0].productionList").ToString();
            List<string> prodList = this.GetTokenValueListInJArray(actualResponseBody, "name");
            FileShareConnector fs = new FileShareConnector();
            var shareName = fs.WipStorageSettings.AllKeys.Contains(productionStore) ? fs.WipStorageSettings[productionStore] : productionStore;
            productionStoreDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetProductionStoreDetails, args: new { name = shareName }).ConfigureAwait(false);
            var fileShare = new FileShareConnector(productionStoreDetails[Constants.WIPKeyName]);
            List<string> productionsList = await fileShare.ListProductionsInShare(shareName);
            Assert.IsTrue(productionsList.All(prodList.Contains), $"Productions List mismatch: \n {string.Join(",", productionsList.Except(prodList).ToArray())} \n {string.Join(",", prodList.Except(productionsList).ToArray())}");
        }

        [Then(@"I verify if all production stores present are listed")]
        public async Task ThenIVerifyIfAllProductionStoresPresentAreListed()
        {
            string actualResponseBody = ((JObject)this.ScenarioContext[Constants.Result]).SelectToken("data[0].productionStoreList").ToString();
            List<string> prodStoreJsonList = this.GetTokenValueListInJArray(actualResponseBody, "name");
            List<string> prodStoreDBList = new List<string>();

            var productionDetails = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetAllProductionStoresDetails).ConfigureAwait(false);
            foreach (var productionItem in productionDetails)
            {
                prodStoreDBList.Add(productionItem[Constants.Name].ToString());
            }

            Assert.IsTrue(prodStoreJsonList.All(prodStoreDBList.Contains), $"Productions List mismatch: \n {string.Join(",", prodStoreJsonList.Except(prodStoreDBList).ToArray())} \n {string.Join(",", prodStoreDBList.Except(prodStoreJsonList).ToArray())}");
        }

        [Then(@"Verify ""(.*)"" is not present in the list of stores returned")]
        public void ThenVerifyInTheListOfStoresReturned(string productionStore)
        {
            string actualResponseBody = ((JObject)this.ScenarioContext[Constants.Result]).SelectToken("data[0].productionStoreList").ToString();
            List<string> prodStoreJsonList = this.GetTokenValueListInJArray(actualResponseBody, "name");
            Assert.IsFalse(prodStoreJsonList.Contains("productionStore"));
        }

        [Then(@"I verify the details of each ""(.*)""")]
        public async Task ThenIVerifyTheDetailsOfEach(string var)
        {
            string jArrayName = var.ToCamelCase() + "List";
            List<string> tokenList = new List<string>();
            if (jArrayName.EqualsIgnoreCase("productionList"))
            {
                tokenList = new List<string> { Constants.Id, Constants.Name, Constants.ProductionStoreId, Constants.Status, Constants.CreatedDateTime, Constants.LastSyncDateTime, Constants.SizeInBytes };
            }
            else if (jArrayName.EqualsIgnoreCase("productionStoreList"))
            {
                tokenList = new List<string> { Constants.Id, Constants.Name, Constants.Region, Constants.ArchiveAllocatedSize, Constants.UserRoleGroupNames, Constants.ManagerRoleGroupNames, Constants.ScaleDownTime, Constants.ScaleUpTimeInterval, Constants.MinimumFreeSize, Constants.MinimumFreeSpace, Constants.OfflineTime, Constants.OnlineTime };
            }

            await this.VerifyJarrayOfobjectsAndSqlData(jArrayName, ((JObject)this.ScenarioContext[Constants.Result]).SelectToken("data[0]." + jArrayName).ToString(), tokenList);
        }

        [When(@"I set the status to ""(.*)"" for ""(.*)"" production")]
        public async Task WhenISetTheStatusToForProduction(string status, string production)
        {
            production = this.WipStorageSettings.AllKeys.Contains(production) ? this.WipStorageSettings[production] : production;
            string prodGUID = await this.GetProdGUIDAsync(production);
            await this.ReadSqlAsync(SqlQueries.UpdateProductionStatusById, args: new { status = status, id = prodGUID }).ConfigureAwait(false);
        }

        [Given(@"I add ""(.*)"" to ""(.*)"" security group for ""(.*)"" production store")]
        public async Task GivenIAddToSecurityGroupForProductionStore(string groupName, string groupCategory, string productionStore)
        {
            var ps = this.WipStorageSettings.AllKeys.Contains(productionStore) ? this.WipStorageSettings[productionStore] : productionStore;
            string psGUID = await this.GetPSGUIDAsync(ps);
            groupName = this.GetGroupNames(groupName);
            await this.GivenIRemoveAllGroupsForBothSecurityGroupForProductionStore(productionStore);
            if (groupCategory.ToLower().Contains("manager"))
            {
                await this.ReadSqlAsync(SqlQueries.UpdateProductionStoreManagerGroup, args: new { managerRoleGroupNames = groupName, id = psGUID }).ConfigureAwait(false);
            }

            if (groupCategory.ToLower().Contains("user"))
            {
                await this.ReadSqlAsync(SqlQueries.UpdateProductionStoreUserGroup, args: new { userRoleGroupNames = groupName, id = psGUID }).ConfigureAwait(false);
            }
        }

        [Given(@"I remove all groups for both security group for ""(.*)"" production store")]
        public async Task GivenIRemoveAllGroupsForBothSecurityGroupForProductionStore(string productionStore)
        {
            var ps = this.WipStorageSettings.AllKeys.Contains(productionStore) ? this.WipStorageSettings[productionStore] : productionStore;
            this.ScenarioContext[Constants.ProductionStoreName] = ps;
            string psGUID = await this.GetPSGUIDAsync(ps);
            await this.ReadSqlAsync(SqlQueries.UpdateProductionStoreManagerGroup, args: new { managerRoleGroupNames = string.Empty, id = psGUID }).ConfigureAwait(false);
            await this.ReadSqlAsync(SqlQueries.UpdateProductionStoreUserGroup, args: new { userRoleGroupNames = string.Empty, id = psGUID }).ConfigureAwait(false);
        }

        [Then(@"I verify if all production stores present are listed for which ""(.*)"" is in user/managed group")]
        public async Task ThenIVerifyIfAllProductionStoresPresentAreListedForWhichIsInUserManagedGroup(string groupName)
        {
            groupName = this.GetGroupNames(groupName);
            var psDetails = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetPSForAGroup, args: new { groupName = "%" + groupName + "%" }).ConfigureAwait(false);
            string actualResponseBody = ((JObject)this.ScenarioContext[Constants.Result]).SelectToken("data[0].productionStoreList").ToString();
            List<string> prodStoreJsonList = this.GetTokenValueListInJArray(actualResponseBody, "name");
            List<string> prodStoreDBList = new List<string>();

            foreach (var productionItem in psDetails)
            {
                prodStoreDBList.Add(productionItem[Constants.Name].ToString());
            }

            Assert.IsTrue(prodStoreJsonList.All(prodStoreDBList.Contains), $"Productions List mismatch: \n {string.Join(",", prodStoreJsonList.Except(prodStoreDBList).ToArray())} \n {string.Join(",", prodStoreDBList.Except(prodStoreJsonList).ToArray())}");
        }

        [StepDefinition(@"I get the list of files and folders before making ""(.*)"" production offline")]
        public async Task WhenIGetTheListOfFilesAndFoldersBeforeMakingProductionOffline(string productionName)
        {
            var productionStore = productionName.Split("_")[0].ToString();
            var prodName = this.WipStorageSettings.AllKeys.Contains(productionName) ? this.WipStorageSettings[productionName] : productionName;
            productionStore = this.WipStorageSettings.AllKeys.Contains(productionStore) ? this.WipStorageSettings[productionStore] : productionStore;
            FileShareConnector fs = await this.CreateWIPConnectorAsync(productionStore).ConfigureAwait(false);
            this.ScenarioContext[Constants.WipListBeforeRestore] = await fs.ListProductionFoldersAndFiles(prodName, productionStore);
            string psGUID = await this.GetPSGUIDAsync(productionStore);
            var productionDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetailsInProductionStore, args: new { name = prodName, productionStoreId = psGUID }).ConfigureAwait(false);
            this.ScenarioContext[Constants.CreatedDateTime] = productionDetails[Constants.CreatedDateTime];
        }

        [Then(@"Verify ""(.*)"" state for productions in DB")]
        public async Task ThenVerifyStateForProductionsInDB(string state)
        {
            var productionDetails = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetAllProductionDetails).ConfigureAwait(false);
            if (this.ScenarioContext.ContainsKey("ProductionStoreName"))
            {
                productionDetails = await this.ReadAllSqlAsDictionaryAsync(SqlQueries.GetProductionDetailsByProductionStore, args: new { productionStoreId = await this.GetPSGUIDAsync(this.ScenarioContext["ProductionStoreName"].ToString()) }).ConfigureAwait(false);
            }

            foreach (var productionItem in productionDetails)
            {
                var status = productionItem[Constants.Status];
                Assert.IsTrue(status.ToString().Equals("Archiving") || status.ToString().Equals("Online"), $"Production Id - {productionItem[Constants.Id]} \n Status - {status} while Archivig");
            }
        }
    }
}
