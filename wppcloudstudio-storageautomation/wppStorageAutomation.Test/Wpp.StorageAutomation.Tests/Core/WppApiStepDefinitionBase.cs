// <copyright file="WppApiStepDefinitionBase.cs" company="Microsoft">
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//    THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//    OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//    ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//    OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace Wpp.StorageAutomation.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Bdd.Core;
    using Bdd.Core.Api.StepDefinitions;
    using Bdd.Core.Utils;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using TechTalk.SpecFlow;
    using Wpp.StorageAutomation.Tests.Entities;
    using Wpp.StorageAutomation.Tests.Executors;

    public class WppApiStepDefinitionBase : ApiStepDefinitionBase
    {
        public readonly NameValueCollection WipStorageSettings = ConfigManager.GetSection("wipStorage") ?? throw new ConfigurationErrorsException("wipStorage");
        public readonly NameValueCollection ArchiveStorageSettings = ConfigManager.GetSection("archiveStorage") ?? throw new ConfigurationErrorsException("archiveStorage");
        public readonly NameValueCollection TestDataStorageSettings = ConfigManager.GetSection("testDataStorage") ?? throw new ConfigurationErrorsException("testDataStorage");
        public readonly NameValueCollection StorageAccountDetails = ConfigManager.GetSection("storageAccountDetails") ?? throw new ConfigurationErrorsException("storageAccountDetails");
        public readonly NameValueCollection UserGroupDetails = ConfigManager.GetSection("userGroupDetails") ?? throw new ConfigurationErrorsException("userGroupDetails");

        public WppApiStepDefinitionBase()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            this.Filepath = location.Replace("Wpp.StorageAutomation.Tests.dll", string.Empty);
            this.Endpoint = ConfigManager.AppSettings["apiBaseUrl"];
            this.RequestHeaders.Add("Content-type", "application/json; charset=UTF-8");
            this.RequestHeaders.Add(Constants.AppAuthToken, "Bearer " + ConfigManager.AppSettings[Constants.Token]);

            // this.Date = DateTime.Now.ToString("MMMdd");
            // this.ScenarioContext[nameof(this.UserDetails)] = new Credentials();
        }

        public readonly string Endpoint;
        public readonly string Filepath;
        public string JsonBodyString;
        public JObject ResponseBody;
        public JObject RequestObject;
        public JArray ResponseArray;
        public JArray RequestArray;
        public object Result = null;

        public Dictionary<string, object> RequestHeaders = new Dictionary<string, object>();

        public async Task<string> GetPSGUIDAsync(string ps)
        {
            string psGUID = await this.ReadSqlScalarAsync<string>(SqlQueries.GetGuidOfPS, args: new { name = ps }).ConfigureAwait(false);
            psGUID = psGUID != null ? psGUID : throw new Exception("Null production store GUID");
            return psGUID;
        }

        public async Task<string> GetProdGUIDAsync(string prod)
        {
            string prodGuid = await this.ReadSqlScalarAsync<string>(SqlQueries.GetGuidOfProd, args: new { name = prod }).ConfigureAwait(false);
            prodGuid = prodGuid != null ? prodGuid : throw new Exception("Null production store GUID");
            return prodGuid;
        }

        public void GetJsonTemplate(string fileName)
        {
            string path = $@"{this.Filepath}TestData\ApiRequestBody\{fileName}.json".GetFullPath();
            this.RequestObject = JObject.Parse(File.ReadAllText(path));
        }

        public JObject GetResponseJsonTemplate(string fileName)
        {
            string path = $@"{this.Filepath}TestData\ApiResponseBody\{fileName}.json".GetFullPath();
            JObject responseBody = JObject.Parse(File.ReadAllText(path));
            return responseBody;
        }

        public async Task<dynamic> PostCallAsync(string url, JObject payload, Dictionary<string, object> headers)
        {
            try
            {
                this.Result = await this.PostAsync<dynamic>(url, payload, headers);
                this.ScenarioContext["ResponseMsg"] = "200 OK";
                return this.Result;
            }
            catch (Flurl.Http.FlurlHttpException e)
            {
                this.ScenarioContext["ResponseMsg"] = e.Message;
                return null;
            }
        }

        public async Task<dynamic> GetCallAsync(string url, Dictionary<string, object> headers)
        {
            try
            {
                this.Result = await this.GetAsync<dynamic>(url, headers);
                this.ScenarioContext["ResponseMsg"] = "200 OK";
                return this.Result;
            }
            catch (Flurl.Http.FlurlHttpException e)
            {
                this.ScenarioContext["ResponseMsg"] = e.Message;
                return null;
            }
        }

        // ========================================================
        public async Task GetResponseDataForAPI()
        {
            var firstRequestResult = JObject.FromObject(this.Result);
            var statusQueryGetUri = JObject.FromObject(this.Result)["statusQueryGetUri"].ToString();
            this.ScenarioContext["statusQueryGetUri"] = statusQueryGetUri;
            do
            {
                this.ResponseBody = await this.GetCallAsync(statusQueryGetUri, this.RequestHeaders);
                this.Result = firstRequestResult;
            }
            while (this.ResponseBody.SelectToken("runtimeStatus").ToString() != "Completed");
        }

        public void CreateJsonRequestBody(string api, Table table)
        {
            var expectedHeaders = table.Rows.ToDictionary(r => r[0], r => r[1]);
            this.GetJsonTemplate(api.ToPascalCase());
            foreach (KeyValuePair<string, string> entry in expectedHeaders)
            {
                var token = this.RequestObject.SelectToken(entry.Key);
                token.Replace(expectedHeaders[entry.Key]);
            }
        }

        public void CreateJsonRequestBody(string api, string key, string value)
        {
            this.GetJsonTemplate(api.ToPascalCase());
            var token = this.RequestObject.SelectToken(key);
            token.Replace(value);
        }

        public Dictionary<string, string> GetResponseTokenValue(string result, string api, Table table)
        {
            var expectedHeaders = table.Rows.ToDictionary(r => r[0], r => r[1]);
            Dictionary<string, string> responseTokenValue = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> entry in expectedHeaders)
            {
                if (api.ToPascalCase() == "CreateProduction")
                {
                    if (result.EqualsIgnoreCase("failed"))
                    {
                        string newKey = entry.Key;
                        responseTokenValue.Add(newKey, entry.Value);
                    }
                    else if (result.EqualsIgnoreCase("success"))
                    {
                        string newKey = "data[0]." + entry.Key;
                        responseTokenValue.Add(newKey, entry.Value);
                    }
                }
                else
                {
                    if (result.EqualsIgnoreCase("failed"))
                    {
                        string newKey = "output." + entry.Key;
                        responseTokenValue.Add(newKey, entry.Value);
                    }
                }
            }

            return responseTokenValue;
        }

        public List<string> GetTokenValueListInJArray(string json, string token)
        {
            List<string> list = new List<string>();
            JArray jsonObject = JArray.Parse(json);
            foreach (JObject content in jsonObject.Children<JObject>())
            {
                foreach (JProperty prop in content.Properties().Where(p => p.Name == token))
                {
                    list.Add(prop.Value.ToString());
                }
            }

            return list;
        }

        public async Task VerifyJarrayOfobjectsAndSqlData(string jArrayName, string json, List<string> tokenList)
        {
            JArray jsonObject = JArray.Parse(json);
            foreach (JObject content in jsonObject.Children<JObject>())
            {
                List<string> actual = new List<string>();
                List<string> expected = new List<string>();
                IDictionary<string, string> queryResult = new Dictionary<string, string>();
                if (jArrayName.EqualsIgnoreCase("productionList"))
                {
                    string identifier = content.SelectToken("name").ToString();
                    string psGUID = await this.GetPSGUIDAsync(this.ScenarioContext["ProductionStoreName"].ToString());
                    queryResult = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetSpecificProductionDetailsInProductionStore, args: new { name = identifier, productionStoreId = psGUID }).ConfigureAwait(false);
                }

                if (jArrayName.EqualsIgnoreCase("productionStoreList"))
                {
                    string identifier = content.SelectToken("name").ToString();
                    queryResult = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetProductionStoreDetails, args: new { name = identifier }).ConfigureAwait(false);
                }

                foreach (string token in tokenList)
                {
                    var actualVal = queryResult[token] == null ? null : queryResult[token].Replace(".0000000000", string.Empty);
                    actualVal = actualVal == string.Empty ? null : actualVal;
                    actual.Add(actualVal);
                    expected.Add(content.SelectToken(token.ToCamelCase()).ToString() != string.Empty ? content.SelectToken(token.ToCamelCase()).ToString() : null);
                }

                Assert.IsTrue(Enumerable.SequenceEqual(actual, expected), $"Values mismatch for {actual.ElementAt(1)}:\n {string.Join(",", actual.Except(expected).ToArray())} \n {string.Join(",", expected.Except(actual).ToArray())}");
            }
        }

        public async Task CreateProdUsingAPIAsync(string containerName, string directory)
        {
            string psGUID = await this.GetPSGUIDAsync(containerName);
            string url = this.Endpoint + string.Format(GenericExtensions.GetResourceValue("CreateProduction", "APIRoutes"), psGUID);
            Table reqData = new Table("Key", "Value");
            reqData.AddRow("tokens[0].productionToken", directory);
            this.CreateJsonRequestBody("CreateProduction", reqData);
            this.RequestObject["directoryTree"][0]["subitems"] = new JArray();
            this.Result = await this.PostCallAsync(url, this.RequestObject, this.RequestHeaders);

            Logger.Info("####################created production#############");
            Logger.Info(url);
            Logger.Info(this.RequestObject.ToString());
            Logger.Info(this.Result.ToString());
        }

        public async Task RestoreProdUsingAPIAsync(string prodName, string productionStore)
        {
            string psGUID = await this.GetPSGUIDAsync(productionStore);
            string prodGUID = await this.ReadSqlScalarAsync<string>(SqlQueries.GetGuidOfProd, args: new { name = prodName }).ConfigureAwait(false);
            string url = this.Endpoint + string.Format(GenericExtensions.GetResourceValue("RestoreProduction", "APIRoutes"), psGUID, prodGUID);
            this.Result = await this.PostCallAsync(url, null, this.RequestHeaders);
            string responseMsg = this.ScenarioContext.Get<string>("ResponseMsg");
            Assert.IsTrue(responseMsg.Contains("200"), $"Response message received is :{responseMsg}");
            await this.GetResponseDataForAPI();
        }

        public async Task<FileShareConnector> CreateWIPConnectorAsync(string psName = "Default")
        {
            IDictionary<string, string> productionStoreDetails = null;
            psName = psName.EqualsIgnoreCase("Default") ? this.ScenarioContext[Constants.ProductionStoreName].ToString() : psName;
            productionStoreDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetProductionStoreDetails, args: new { name = psName }).ConfigureAwait(false);
            FileShareConnector fileShare = new FileShareConnector(productionStoreDetails[Constants.WIPKeyName]);
            return fileShare;
        }

        public async Task<BlobConnector> CreateArcConnectorAsync(string psName = "Default")
        {
            IDictionary<string, string> productionStoreDetails = null;
            psName = psName.EqualsIgnoreCase("Default") ? this.ScenarioContext[Constants.ProductionStoreName].ToString() : psName;
            productionStoreDetails = await this.ReadSqlAsStringDictionaryAsync(SqlQueries.GetProductionStoreDetails, args: new { name = psName }).ConfigureAwait(false);
            BlobConnector blobConnector = new BlobConnector(productionStoreDetails[Constants.ArchiveKeyName]);
            return blobConnector;
        }

        public string GetGroupNames(string groupName)
        {
            string cellValue = string.Empty;
            string[] values = groupName.Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                cellValue = cellValue + (this.WipStorageSettings.AllKeys.Contains(values[i]) ? this.WipStorageSettings[values[i]] : values[i]).Trim() + ",";
            }

            return cellValue;
        }

        public string GetUserGroupNames(string productionStore, string groupType = "")
        {
            var groups = GenericExtensions.GetResourceValue($"{productionStore}{groupType.ToLower()}", "UserGroupDetails");
            groups = groups == null ? GenericExtensions.GetResourceValue($"Default{groupType.ToLower()}", "UserGroupDetails") : groups;
            return groups;
        }
    }
}
