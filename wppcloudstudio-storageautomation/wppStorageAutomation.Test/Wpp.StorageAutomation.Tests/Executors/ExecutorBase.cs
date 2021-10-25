// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecutorBase.cs" company="Microsoft">
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
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;

    using Bdd.Core;
    using Bdd.Core.Utils;

    public class ExecutorBase
    {
        protected readonly string filepath;

        public ExecutorBase()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            this.filepath = location.Replace("Wpp.StorageAutomation.Tests.dll", string.Empty);
        }

        public NameValueCollection GetConfigDetails(string section)
        {
           return ConfigManager.GetSection(section) ?? throw new ConfigurationErrorsException(section);
        }

        public string GetValueFromKeyVault(string key)
        {
            return KeyVaultHelper.GetKeyVaultSecretAsync(key).GetAwaiter().GetResult().Value;
        }
    }
 }
