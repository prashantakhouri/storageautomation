// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataMovementConnector.cs" company="Microsoft">
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
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Threading;
    using System.Threading.Tasks;

    using Bdd.Core;
    using Bdd.Core.Utils;

    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.Storage.DataMovement;
    using Microsoft.Azure.Storage.File;

    public class DataMovementConnector : ExecutorBase
    {
        public readonly NameValueCollection TestDataStorageSettings = ConfigManager.GetSection("testDataStorage") ?? throw new ConfigurationErrorsException("testDataStorage");
        public readonly NameValueCollection WipStorageSettings = ConfigManager.GetSection("wipStorage") ?? throw new ConfigurationErrorsException("wipStorage");
        public CloudBlobClient BlobClient;
        public CloudFileClient FileClient;

        public DataMovementConnector()
        {
            // ======= test data SA=====
            // var connectionString1 = "DefaultEndpointsProtocol=https;AccountName=sawppweudevstauarcprem01;AccountKey=JRJ3bFWDovAFrwLBtXdQiQF79l+zmHx0Modv9eDVcURuu7TUJ/SeNFaAQrNlYJD+POgkEm5mrIfMlu2sSZKLXA==;EndpointSuffix=core.windows.net";
            var connectionString1 = KeyVaultHelper.GetKeyVaultSecretAsync("kv-sa-testdatasetup").GetAwaiter().GetResult().Value;

            var account = CloudStorageAccount.Parse(connectionString1);
            this.BlobClient = account.CreateCloudBlobClient();

            // ======= WIP SA=====
            // var c2 = "DefaultEndpointsProtocol=https;AccountName=sawppweudevstauwipprem01;AccountKey=6xEffFgUVQdyvEFWmErs5oWl1B6/IGULWttK1z8/EFBuavuKFQbyyIbzEG0VL9RUxvKrnbBsml71VuKqAYhYgg==;EndpointSuffix=core.windows.net";
            var connectionString2 = KeyVaultHelper.GetKeyVaultSecretAsync("kv-sa-sawpptestwip01").GetAwaiter().GetResult().Value;

            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString2);
            this.FileClient = cloudStorageAccount.CreateCloudFileClient();
        }

        public DataMovementConnector(string testDataSAKey, string wipSAKey)
        {
            // ======= test data SA=====
            // var connectionString1 = "DefaultEndpointsProtocol=https;AccountName=sawppweudevstauarcprem01;AccountKey=JRJ3bFWDovAFrwLBtXdQiQF79l+zmHx0Modv9eDVcURuu7TUJ/SeNFaAQrNlYJD+POgkEm5mrIfMlu2sSZKLXA==;EndpointSuffix=core.windows.net";
            var connectionString1 = testDataSAKey.ContainsIgnoreCase("sawppcsweutestdatastore") ? testDataSAKey : KeyVaultHelper.GetKeyVaultSecretAsync(testDataSAKey).GetAwaiter().GetResult().Value;

            var account = CloudStorageAccount.Parse(connectionString1);
            this.BlobClient = account.CreateCloudBlobClient();

            // ======= WIP SA=====
            // var c2 = "DefaultEndpointsProtocol=https;AccountName=sawppweudevstauwipprem01;AccountKey=6xEffFgUVQdyvEFWmErs5oWl1B6/IGULWttK1z8/EFBuavuKFQbyyIbzEG0VL9RUxvKrnbBsml71VuKqAYhYgg==;EndpointSuffix=core.windows.net";
            var connectionString2 = KeyVaultHelper.GetKeyVaultSecretAsync(wipSAKey).GetAwaiter().GetResult().Value;

            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString2);
            this.FileClient = cloudStorageAccount.CreateCloudFileClient();
        }

        public async Task MoveTestDataFromBlobToBlob()
        {
            var connectionString1 = " ";
            var account1 = CloudStorageAccount.Parse(connectionString1);
            var blobClient1 = account1.CreateCloudBlobClient();
            var container1 = blobClient1.GetContainerReference("testfileshare");
            var rootBlobDirectory1 = container1.GetDirectoryReference(string.Empty);
            var productionBlobDirectory1 = rootBlobDirectory1.GetDirectoryReference("Sample Large Production 25GB");

            var connectionString2 = " ";
            var account2 = CloudStorageAccount.Parse(connectionString2);
            var blobClient2 = account2.CreateCloudBlobClient();
            var container2 = blobClient2.GetContainerReference("restore-ps");
            await container2.CreateIfNotExistsAsync();
            var rootBlobDirectory2 = container2.GetDirectoryReference(string.Empty);
            var productionBlobDirectory2 = rootBlobDirectory2.GetDirectoryReference("Test-Production-Large-25GB");

            TransferCheckpoint checkpoint = null;
            DirectoryTransferContext context = new DirectoryTransferContext(checkpoint);
            context.ProgressHandler = new Progress<TransferStatus>((progress) =>
            {
            });

            var cancellationSource = new CancellationTokenSource();

            var options = new CopyDirectoryOptions()
            {
                Recursive = true,
            };
            await TransferManager.CopyDirectoryAsync(productionBlobDirectory1, productionBlobDirectory2, CopyMethod.ServiceSideSyncCopy, options, context, cancellationSource.Token);
        }

        public async Task MoveTestData()
        {
            BlobConnector bc = new BlobConnector("testDataStorage");
            var containerList = await bc.ListContainersAsync();
            foreach (string containerName in containerList)
            {
                var container = this.BlobClient.GetContainerReference(containerName);
                var rootBlobDirectory = container.GetDirectoryReference(string.Empty);

                var fileShare = this.FileClient.GetShareReference(containerName);

                // If Production store already exists , data will not be copied: Can be removed if needed
                if (!await fileShare.ExistsAsync())
                {
                    fileShare.CreateIfNotExists();
                    var rootShareDir = fileShare.GetRootDirectoryReference();

                    var directoryList = await bc.ListAllLevel1FoldersInAContainer(containerName);

                    if (this.TestDataStorageSettings["UseLargeSizeProductionsForTesting"].ToString().EqualsIgnoreCase("No"))
                    {
                        directoryList.RemoveAll((x) => x.ContainsIgnoreCase("Large"));
                    }

                    foreach (string directory in directoryList)
                    {
                        var blobDirectory = rootBlobDirectory.GetDirectoryReference(directory);
                        var fileShareDirectory = rootShareDir.GetDirectoryReference(directory);

                        // If Production already exists , data will not be copied : Can be removed if needed
                        if (!await fileShareDirectory.ExistsAsync())
                        {
                            await fileShareDirectory.CreateIfNotExistsAsync();
                            await this.TransferFilesAsync(fileShareDirectory, blobDirectory).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        public async Task TransferFilesAsync(CloudFileDirectory fileDirectory, CloudBlobDirectory blobDirectory)
        {
            TransferCheckpoint checkpoint = null;
            DirectoryTransferContext context = new DirectoryTransferContext(checkpoint);
            context.ProgressHandler = new Progress<TransferStatus>((progress) =>
            {
            });

            var cancellationSource = new CancellationTokenSource();

            var options = new CopyDirectoryOptions()
            {
                Recursive = true,
            };
            await TransferManager.CopyDirectoryAsync(blobDirectory, fileDirectory, CopyMethod.ServiceSideSyncCopy, options, context, cancellationSource.Token);
        }
    }
 }
