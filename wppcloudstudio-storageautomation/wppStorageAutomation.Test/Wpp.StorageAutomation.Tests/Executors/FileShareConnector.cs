// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileShareConnector.cs" company="Microsoft">
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

    using Bdd.Core;
    using Bdd.Core.Utils;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.File;

    using NUnit.Framework;

    using Wpp.StorageAutomation.Tests.Core;
    using Wpp.StorageAutomation.Tests.Entities;

    public class FileShareConnector : ExecutorBase
    {
        public readonly NameValueCollection WipStorageSettings = ConfigManager.GetSection("wipStorage") ?? throw new ConfigurationErrorsException("wipStorage");
        private readonly string connectionString;
        private readonly List<string> fileListHierarchy = new List<string>();
        private readonly CloudFileClient fileClient;
        private readonly List<string> fileShareList = new List<string>();
        private readonly List<string> fileListFromAllFileShares = new List<string>();
        private readonly List<string> productionsInFileShareList = new List<string>();
        private CloudFileDirectory rootDir;
        private string shareName;

        public FileShareConnector()
        {
            // implemetation ignored
        }

        public FileShareConnector(string key)
        {
            this.connectionString = KeyVaultHelper.GetKeyVaultSecretAsync(key).GetAwaiter().GetResult().Value;
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(this.connectionString);
            this.fileClient = cloudStorageAccount.CreateCloudFileClient();
        }

        //--------------------------------------------------------------------------
        // Check if a production is present in production store
        // fileShareName = productionStoreName
        // directory = production
        //--------------------------------------------------------------------------
        public async Task<bool> SearchDirectoryInShare(string directory, string fileshareName = "Default")
        {
            this.shareName = fileshareName.EqualsIgnoreCase("Default") ? this.WipStorageSettings["ProductionStore1"] : fileshareName;
            var fileShare = this.fileClient.GetShareReference(this.shareName);
            this.rootDir = fileShare.GetRootDirectoryReference();
            bool productionExists = await this.rootDir.GetDirectoryReference(directory).ExistsAsync();
            return productionExists;
        }

        //--------------------------------------------------------------------------
        // List file sharesNames  ( List all Production store names from WIP storage)
        //--------------------------------------------------------------------------
        public async Task<List<string>> ListFileSharesAsync()
        {
            var segment = await this.fileClient.ListSharesSegmentedAsync(null);
            foreach (var item in segment.Results)
            {
                Console.WriteLine(item.Name);
                this.fileShareList.Add(item.Name);
            }

            return this.fileShareList;
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Hierarchy list with in given Production Store( List all folder subfolder within a production store)
        //------------------------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListShareHierarchy(string fileshareName = "Default")
        {
            this.shareName = fileshareName.EqualsIgnoreCase("Default") ? this.WipStorageSettings["ProductionStore1"] : fileshareName;
            var fileShare = this.fileClient.GetShareReference(this.shareName);
            this.rootDir = fileShare.GetRootDirectoryReference();
            return await this.ListHierarchy(this.rootDir);
        }

        //-----------------------------------------------------------------------------------------------------------------
        // List of productions in a production store
        //------------------------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListProductionsInShare(string fileshareName = "Default")
        {
            this.shareName = fileshareName.EqualsIgnoreCase("Default") ? this.WipStorageSettings["ProductionStore1"] : fileshareName;
            var fileShare = this.fileClient.GetShareReference(this.shareName);
            this.rootDir = fileShare.GetRootDirectoryReference();
            return await this.ListLevel1Directories(this.rootDir);
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Hierarchy list with in All Production Store( List all folder subfolder From All production store)
        //------------------------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListAllShareHierarchy()
        {
            await this.ListFileSharesAsync();
            List<string> listFromAFileShare = new();
            foreach (string fileshare in this.fileShareList)
            {
                listFromAFileShare = await this.ListShareHierarchy(fileshare);
                this.fileListFromAllFileShares.AddRange(listFromAFileShare);
            }

            return this.fileListFromAllFileShares;
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Hierarchy list with in given Production( List all folder subfolder within a production store)
        // fileShareName = productionStoreName
        // directory = production
        //------------------------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListProductionHierarchy(string productionName, string fileShareName = "Default")
        {
            this.shareName = fileShareName.EqualsIgnoreCase("Default") ? this.WipStorageSettings["ProductionStore1"] : fileShareName;
            var fileShare = this.fileClient.GetShareReference(this.shareName);
            this.rootDir = fileShare.GetRootDirectoryReference();
            var directory = this.WipStorageSettings.AllKeys.Contains(productionName) ? this.WipStorageSettings[productionName] : productionName;
            CloudFileDirectory fileitem = this.rootDir.GetDirectoryReference(directory);
            return await this.ListHierarchy(fileitem);
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Hierarchy list with in given Production( List all folder subfolder within a production store)
        // fileShareName = productionStoreName
        // directory = production
        //------------------------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListProductionFoldersAndFiles(string productionName, string fileShareName = "Default")
        {
            this.shareName = fileShareName.EqualsIgnoreCase("Default") ? this.WipStorageSettings["ProductionStore1"] : fileShareName;
            var fileShare = this.fileClient.GetShareReference(this.shareName);
            this.rootDir = fileShare.GetRootDirectoryReference();
            var directory = this.WipStorageSettings.AllKeys.Contains(productionName) ? this.WipStorageSettings[productionName] : productionName;
            CloudFileDirectory fileitem = this.rootDir.GetDirectoryReference(directory);
            return await this.ListFoldersAndFiles(fileitem);
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Hierarchy list with in given Directory in File Share
        //------------------------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListFoldersAndFiles(IListFileItem list)
        {
            CloudFileDirectory fileDirectory = (CloudFileDirectory)list;
            List<IListFileItem> fileList = new List<IListFileItem>();
            FileContinuationToken token = null;
            if ((await fileDirectory.ExistsAsync()).ToString().ToLower() == "true")
            {
                do
                {
                    FileResultSegment resultSegment = await fileDirectory.ListFilesAndDirectoriesSegmentedAsync(token);
                    fileList.AddRange(resultSegment.Results);
                    token = resultSegment.ContinuationToken;
                }
                while (token != null);
            }
            else
            {
                throw new Exception($"{fileDirectory.Name} Production does not exist in WIP");
            }

            // Print all files/directories in the folder.
            foreach (IListFileItem listItem in fileList)
            {
                this.fileListHierarchy.Add(listItem.Uri.LocalPath);

                // listItem type will be CloudFile or CloudFileDirectory.
                if (listItem.GetType() == typeof(Microsoft.WindowsAzure.Storage.File.CloudFileDirectory))
                {
                    await this.ListFoldersAndFiles(listItem);
                }
            }

            return this.fileListHierarchy;
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Hierarchy list with in given Directory in File Share
        //------------------------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListHierarchy(IListFileItem list)
        {
            CloudFileDirectory fileDirectory = (CloudFileDirectory)list;
            List<IListFileItem> fileList = new List<IListFileItem>();
            FileContinuationToken token = null;
            do
            {
                FileResultSegment resultSegment = await fileDirectory.ListFilesAndDirectoriesSegmentedAsync(token);
                fileList.AddRange(resultSegment.Results);
                token = resultSegment.ContinuationToken;
            }
            while (token != null);

            // Print all files/directories in the folder.
            foreach (IListFileItem listItem in fileList)
            {
                // Console.WriteLine(listItem.Uri.LocalPath);
                // listItem type will be CloudFile or CloudFileDirectory.
                if (listItem.GetType() == typeof(Microsoft.WindowsAzure.Storage.File.CloudFileDirectory))
                {
                    await this.ListHierarchy(listItem);
                }
                else
                {
                    this.fileListHierarchy.Add(listItem.Uri.LocalPath.Replace($"/{this.shareName}/", string.Empty));
                }
            }

            return this.fileListHierarchy;
        }

        //-----------------------------------------------------------------------------------------------------------------
        // List of directories at level 1
        //------------------------------------------------------------------------------------------------------------------
        public async Task<List<string>> ListLevel1Directories(IListFileItem list)
        {
            CloudFileDirectory fileDirectory = (CloudFileDirectory)list;
            List<IListFileItem> fileList = new List<IListFileItem>();
            FileContinuationToken token = null;
            do
            {
                FileResultSegment resultSegment = await fileDirectory.ListFilesAndDirectoriesSegmentedAsync(token);
                fileList.AddRange(resultSegment.Results);
                token = resultSegment.ContinuationToken;
            }
            while (token != null);

            // Print all files/directories in the folder.
            foreach (IListFileItem listItem in fileList)
            {
                this.productionsInFileShareList.Add(listItem.Uri.LocalPath.Replace($"/{this.shareName}/", string.Empty));
            }

            return this.productionsInFileShareList;
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Delete Production in given Fileshare(Production Store)
        // fileShareName = productionStoreName
        // directory = production
        //------------------------------------------------------------------------------------------------------------------
        public async Task DeleteProduction(string directory, string fileShareName = "Default")
        {
            this.shareName = fileShareName.EqualsIgnoreCase("Default") ? this.WipStorageSettings["ProductionStore1"] : fileShareName;
            var fileShare = this.fileClient.GetShareReference(this.shareName);
            this.rootDir = fileShare.GetRootDirectoryReference();
            await this.DeleteDirectoryAsync(this.rootDir.GetDirectoryReference(directory));
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Delete Directories and files from Azure file share
        //------------------------------------------------------------------------------------------------------------------
        public async Task DeleteDirectoryAsync(CloudFileDirectory directory)
        {
            while ((await directory.ExistsAsync()).ToString().ToLower() == "true")
            {
                List<IListFileItem> fileList = new List<IListFileItem>();
                FileContinuationToken token = null;
                do
                {
                    FileResultSegment resultSegment = await directory.ListFilesAndDirectoriesSegmentedAsync(token);
                    fileList.AddRange(resultSegment.Results);
                    token = resultSegment.ContinuationToken;
                }
                while (token != null);

                // Delete all files/directories in the folder.
                foreach (IListFileItem listItem in fileList)
                {
                    switch (listItem)
                    {
                        case CloudFile file:
                            await file.DeleteAsync();
                            break;
                        case CloudFileDirectory dir:
                            await this.DeleteDirectoryAsync(dir);
                            break;
                    }
                }

                await directory.DeleteAsync();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Upload File to a file share
        //------------------------------------------------------------------------------------------------------------------
        public async Task UploadFile(string fileshareName, string directory, string fileName)
        {
            this.shareName = fileshareName.EqualsIgnoreCase("Default") ? this.WipStorageSettings["ProductionStore1"] : fileshareName;
            var fileShare = this.fileClient.GetShareReference(this.shareName);
            this.rootDir = fileShare.GetRootDirectoryReference();
            CloudFileDirectory fileitem = this.rootDir.GetDirectoryReference(directory);
            CloudFile file = fileitem.GetFileReference(fileName);

            // Open a stream from a local file.
            string path = $@"{this.filepath}TestData\TestFiles\{fileName}".GetFullPath();
            Stream fileStream = File.OpenRead(path);

            // Upload the file to Azure.
            await file.UploadFromStreamAsync(fileStream);
            fileStream.Dispose();
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Edit File in file share
        //------------------------------------------------------------------------------------------------------------------
        public async Task<string> EditFile(string fileshareName, string directory, string fileName)
        {
            this.shareName = fileshareName.EqualsIgnoreCase("Default") ? this.WipStorageSettings["ProductionStore1"] : fileshareName;
            var fileShare = this.fileClient.GetShareReference(this.shareName);
            this.rootDir = fileShare.GetRootDirectoryReference();
            CloudFile file = this.rootDir.GetDirectoryReference(directory).GetFileReference(fileName);

            string azure_file_text = await file.DownloadTextAsync() + $"New random line added: {new Random().Next(1000)}";
            await file.UploadTextAsync(azure_file_text);
            return azure_file_text;
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Delete File in file share
        //------------------------------------------------------------------------------------------------------------------
        public async Task DeleteFileInFileShareAsync(string fileshareName, string directory, string fileName)
        {
            this.shareName = fileshareName.EqualsIgnoreCase("Default") ? this.WipStorageSettings["ProductionStore1"] : fileshareName;
            var fileShare = this.fileClient.GetShareReference(this.shareName);
            this.rootDir = fileShare.GetRootDirectoryReference();
            CloudFile file = this.rootDir.GetDirectoryReference(directory).GetFileReference(fileName);
            await file.DeleteAsync().ConfigureAwait(false);
        }

        //-----------------------------------------------------------------------------------------------------------------
        // Delete Folder in file share
        //------------------------------------------------------------------------------------------------------------------
        public async Task DeleteFolderInFileShareAsync(string fileshareName, string directory, string directoryName)
        {
            this.shareName = fileshareName.EqualsIgnoreCase("Default") ? this.WipStorageSettings["ProductionStore1"] : fileshareName;
            var fileShare = this.fileClient.GetShareReference(this.shareName);
            this.rootDir = fileShare.GetRootDirectoryReference();
            CloudFileDirectory dir = this.rootDir.GetDirectoryReference(directory).GetDirectoryReference(directoryName);
            await this.DeleteDirectoryAsync(dir).ConfigureAwait(false);
        }

        //------------------------------------------------------------------------------------------------
        // Delete a file share
        //------------------------------------------------------------------------------------------------
        public async Task DeleteFileShareAsync(string fileShareName)
        {
            var fileShare = this.fileClient.GetShareReference(fileShareName);

            try
            {
                await fileShare.DeleteAsync();
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}", e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
            }
        }

        //-------------------------------------------------
        // Delete All File Shares
        //-------------------------------------------------
        public async Task DeleteAllFileShareAsync()
        {
            if (this.fileShareList.Count == 0)
            {
                await this.ListFileSharesAsync();
            }

            foreach (string filsShareName in this.fileShareList)
            {
                // if (!filsShareName.EqualsIgnoreCase("productionstore-a"))
                // {
                await this.DeleteFileShareAsync(filsShareName);

                // }
            }
        }
    }
}