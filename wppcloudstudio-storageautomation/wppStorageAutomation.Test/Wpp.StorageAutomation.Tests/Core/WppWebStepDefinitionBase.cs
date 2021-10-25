// <copyright file="WppWebStepDefinitionBase.cs" company="Microsoft">
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
    using Bdd.Core.Web.StepDefinitions;

    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using TechTalk.SpecFlow;
    using Wpp.StorageAutomation.Tests.Entities;
    using Wpp.StorageAutomation.Tests.Executors;

    public class WppWebStepDefinitionBase : WebStepDefinitionBase
    {
        public readonly NameValueCollection WipStorageSettings = ConfigManager.GetSection("wipStorage") ?? throw new ConfigurationErrorsException("wipStorage");
        public readonly NameValueCollection ArchiveStorageSettings = ConfigManager.GetSection("archiveStorage") ?? throw new ConfigurationErrorsException("archiveStorage");
        public readonly NameValueCollection TestDataStorageSettings = ConfigManager.GetSection("testDataStorage") ?? throw new ConfigurationErrorsException("testDataStorage");
        public readonly NameValueCollection StorageAccountDetails = ConfigManager.GetSection("storageAccountDetails") ?? throw new ConfigurationErrorsException("storageAccountDetails");
        public readonly NameValueCollection UserGroupDetails = ConfigManager.GetSection("userGroupDetails") ?? throw new ConfigurationErrorsException("userGroupDetails");

        public WppWebStepDefinitionBase()
        {
            // dummy line
        }

        public async Task<string> GetProdGUIDAsync(string prod)
        {
            string prodGuid = await this.ReadSqlScalarAsync<string>(SqlQueries.GetGuidOfProd, args: new { name = prod }).ConfigureAwait(false);
            prodGuid = prodGuid != null ? prodGuid : throw new Exception("Null production store GUID");
            return prodGuid;
        }
    }
}
