// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobConnector.cs" company="Microsoft">
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

    using Wpp.StorageAutomation.Tests.Core;
    using Wpp.StorageAutomation.Tests.Entities;

    public class BlobConnector : ExecutorBase
    {
        public readonly NameValueCollection ArchiveStorageSettings = ConfigManager.GetSection("archiveStorage") ?? throw new ConfigurationErrorsException("archiveStorage");
        private readonly string connectionString;
        private readonly List<string> blobListInAContainer = new List<string>();
        private readonly List<string> blobListFromAllContainers = new List<string>();
        private List<string> containerList = new List<string>();
        private BlobServiceClient blobServiceClient;
        private BlobContainerClient container;

        public BlobConnector()
        {
            // implementation ignored
        }

        public BlobConnector(string key)
        {
            this.connectionString = key.ContainsIgnoreCase("sawppcsweutestdatastore") ? key : KeyVaultHelper.GetKeyVaultSecretAsync(key).GetAwaiter().GetResult().Value;
            this.blobServiceClient = new BlobServiceClient(this.connectionString);
        }

        //--------------------------------------------------------------------------
        // List Containers  ( List all Production store names from Archive storage)
        //--------------------------------------------------------------------------
        public async Task<List<string>> ListContainersAsync(string prefix = null)
        {
            try
            {
                // Call the listing operation and enumerate the result segment.
                var resultSegment =
                    this.blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata, prefix, default)
                    .AsPages();
                await foreach (Azure.Page<BlobContainerItem> containerPage in resultSegment)
                {
                    foreach (BlobContainerItem containerItem in containerPage.Values)
                    {
                        Console.WriteLine("Container name: {0}", containerItem.Name);
                        this.containerList.Add(containerItem.Name);
                    }
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            return this.containerList;
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Hierarchy list with in given Production Store( List all folder subfolder within a production store)
        //------------------------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListAllBlobsInGivenContainer(string containerName, string prefix = null)
        {
            this.container = this.blobServiceClient.GetBlobContainerClient(containerName);
            List<string> listOfBlobsInContainer = await this.ListAllBlobsInAContainer(prefix);
            return listOfBlobsInContainer;
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Hierarchy list with in a Production Store already configured( List all folder subfolder within a production store)
        //------------------------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListAllBlobsInAContainer(string prefix)
        {
            // this.container = this.blobServiceClient.GetBlobContainerClient(containerName);
            try
            {
                // Call the listing operation and return pages of the specified size.
                var resultSegment = this.container.GetBlobsByHierarchyAsync(prefix: prefix, delimiter: "/")
                    .AsPages();

                // Enumerate the blobs returned for each page.
                await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
                {
                    // A hierarchical listing may return both virtual directories and blobs.
                    foreach (BlobHierarchyItem blobhierarchyItem in blobPage.Values)
                    {
                        if (blobhierarchyItem.IsPrefix)
                        {
                            // Write out the prefix of the virtual directory.
                            Console.WriteLine("Virtual directory prefix: {0}", blobhierarchyItem.Prefix);

                            // this.blobListInAContainer.Add(blobhierarchyItem.Prefix.TrimEnd('/'));

                            // Call recursively with the prefix to traverse the virtual directory.
                            await this.ListAllBlobsInAContainer(blobhierarchyItem.Prefix);
                        }
                        else
                        {
                            // Write out the name of the blob.
                            Console.WriteLine("Blob name: {0}", blobhierarchyItem.Blob.Name);
                            this.blobListInAContainer.Add(blobhierarchyItem.Blob.Name);
                        }
                    }

                    Console.WriteLine();
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            return this.blobListInAContainer;
        }

        //-----------------------------------------------------------------------------------------------------------------
        // List All Productions
        //------------------------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListAllLevel1FoldersInAContainer(string containerName)
        {
            List<string> containerListL1 = new List<string>();
            this.container = this.blobServiceClient.GetBlobContainerClient(containerName);
            try
            {
                // Call the listing operation and return pages of the specified size.
                var resultSegment = this.container.GetBlobsByHierarchyAsync(prefix: null, delimiter: "/")
                    .AsPages();

                // Enumerate the blobs returned for each page.
                await foreach (Azure.Page<BlobHierarchyItem> blobPage in resultSegment)
                {
                    // A hierarchical listing may return both virtual directories and blobs.
                    foreach (BlobHierarchyItem blobhierarchyItem in blobPage.Values)
                    {
                        if (blobhierarchyItem.IsPrefix)
                        {
                            // Write out the prefix of the virtual directory.
                            Console.WriteLine("Virtual directory prefix: {0}", blobhierarchyItem.Prefix);
                            containerListL1.Add(blobhierarchyItem.Prefix.TrimEnd('/'));
                        }
                    }

                    Console.WriteLine();
                }
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            return containerListL1;
        }

        //--------------------------------------------------------------------------------------------------
        // Hierarchy list of all files across all PS ( List all folder subfolder within all production store)
        //---------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListAllBlobsInAllContainers()
        {
            List<string> containers = await this.ListContainersAsync(null);
            foreach (string containerName in containers)
            {
                this.container = this.blobServiceClient.GetBlobContainerClient(containerName);
                List<string> listOfBlobsInContainer = await this.ListAllBlobsInAContainer(null);
                this.blobListFromAllContainers.AddRange(listOfBlobsInContainer);
            }

            return this.blobListFromAllContainers;
        }

        //------------------------------------------------------------------------------------------------
        // Delete a folder in container
        //------------------------------------------------------------------------------------------------
        public async Task DeleteFolderInContainerAsync(string containerName, string folderName)
        {
            ////var folderPath = folderName + "/" + folderName + ".metadata";
            var folderPath = folderName + "/";
            this.container = this.blobServiceClient.GetBlobContainerClient(containerName);
            List<string> listOfBlobsInContainer = await this.ListAllBlobsInAContainer(folderPath);
            foreach (var item in listOfBlobsInContainer)
            {
                BlobClient blobClient = this.container.GetBlobClient(item);
                try
                {
                    // Delete the specified production inside productionstore
                    await blobClient.DeleteIfExistsAsync();
                }
                catch (RequestFailedException e)
                {
                    Console.WriteLine("HTTP error code {0}: {1}", e.Status, e.ErrorCode);
                    Console.WriteLine(e.Message);
                }
            }
        }

        //------------------------------------------------------------------------------------------------
        // Delete a container
        //------------------------------------------------------------------------------------------------
        public async Task DeleteContainerAsync(string containerName)
        {
            this.container = this.blobServiceClient.GetBlobContainerClient(containerName);

            try
            {
                // Delete the specified container and handle the exception.
                await this.container.DeleteAsync();
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}", e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
            }
        }

        //-------------------------------------------------
        // Delete All container
        //-------------------------------------------------
        public async Task DeleteAllContainerAsync()
        {
            if (this.containerList.Count == 0)
            {
                await this.ListContainersAsync(null);
            }

            foreach (string containerN in this.containerList)
            {
                await this.DeleteContainerAsync(containerN);
            }
        }

        //-------------------------------------------------
        // Read File Content
        //-------------------------------------------------
        public async Task<string> ReadBlobAsync(string containerName, string fileName)
        {
            this.container = this.blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = this.container.GetBlobClient(fileName);
            string content = string.Empty;

            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                using (var streamReader = new StreamReader(response.Value.Content))
                {
                    content = streamReader.ReadToEnd();
                }
            }

            return content;
        }
    }
}
