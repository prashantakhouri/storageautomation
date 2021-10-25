// <copyright file="Archiver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.Azure.Storage.File;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wpp.StorageAutomation.ArchiveQueueStorage.Contract;
using Wpp.StorageAutomation.ArchiveQueueStorage.Models;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Common.Constants;
using Wpp.StorageAutomation.Entities.Models;

namespace Wpp.StorageAutomation.ArchiveQueueStorage.Concrete
{
    /// <summary>
    /// Archiver.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.ArchiveQueueStorage.Contract.IArchiver" />
    public class Archiver : IArchiver
    {
        private readonly IProductionRepository productionRepository;
        private readonly IProductionStoreRepository productionStoreRepository;
        private readonly IStorageAccountConfig storageAccountConfig;
        private readonly ICloudStorageUtility cloudStorageUtility;
        private readonly ILogger<Archiver> log;
        private readonly string stateDurationTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Archiver" /> class.
        /// </summary>
        /// <param name="productionRepository">The production repository.</param>
        /// <param name="productionStoreRepository">The production store repository.</param>
        /// <param name="storageAccountConfig">The storage account configuration.</param>
        /// <param name="cloudStorageUtility">The cloud storage utility.</param>
        /// <param name="log">The log.</param>
        public Archiver(
            IProductionRepository productionRepository,
            IProductionStoreRepository productionStoreRepository,
            IStorageAccountConfig storageAccountConfig,
            ICloudStorageUtility cloudStorageUtility,
            ILogger<Archiver> log)
        {
            this.productionRepository = productionRepository;
            this.productionStoreRepository = productionStoreRepository;
            this.storageAccountConfig = storageAccountConfig;
            this.cloudStorageUtility = cloudStorageUtility;
            this.log = log;
            this.stateDurationTime = Environment.GetEnvironmentVariable("StateDurationTime");
        }

        /// <summary>
        /// Gets or sets the account storage wip.
        /// </summary>
        /// <value>
        /// The account storage wip.
        /// </value>
        private CloudStorageAccount AccountStorageWip { get; set; }

        /// <summary>
        /// Gets or sets the account storage archive.
        /// </summary>
        /// <value>
        /// The account storage archive.
        /// </value>
        private CloudStorageAccount AccountStorageArchive { get; set; }

        /// <inheritdoc/>
        public async Task ArchiveProductionAsync(string productionId)
        {
            this.log.LogInformation($"Archiver: Entering...");
            var startTime = DateTime.UtcNow;
            var production = await this.productionRepository.GetProductionById(productionId);
            if (production == null)
            {
                throw new DirectoryNotFoundException($"Production Id : {productionId} entry was not found in production table.");
            }

            var prodStore = await this.productionStoreRepository.GetProductionStoreById(production.ProductionStoreId);
            if (prodStore == null)
            {
                throw new DirectoryNotFoundException($"Production Store Id : {production.ProductionStoreId} entry was not found in production store table.");
            }

            bool isValid = this.IsValidProduction(production, Convert.ToDouble(this.stateDurationTime));

            if (!isValid)
            {
                throw new InvalidOperationException($"Production: {production.Name} is either in offline state or archival already in progress.");
            }

            await this.productionRepository.UpdateStatusOfProduction(productionId, DirectoryStatus.Online.ToString(), DirectoryStatus.Archiving.ToString());

            try
            {
                var wipConnectionString = this.storageAccountConfig.GetStorageConnectionString(prodStore.WipkeyName);
                var archiveConnectionString = this.storageAccountConfig.GetStorageConnectionString(prodStore.ArchiveKeyName);

                this.AccountStorageWip = CloudStorageAccount.Parse(wipConnectionString);
                this.AccountStorageArchive = CloudStorageAccount.Parse(archiveConnectionString);

                var fileClient = this.AccountStorageWip.CreateCloudFileClient();
                var wipShare = fileClient.GetShareReference(prodStore.Name);
                var rootShareDir = wipShare.GetRootDirectoryReference();
                var fileDirectory = rootShareDir.GetDirectoryReference(production.Name);

                var blobClient = this.AccountStorageArchive.CreateCloudBlobClient();
                var archiveContainer = blobClient.GetContainerReference(prodStore.Name);
                var rootBlobDirectory = archiveContainer.GetDirectoryReference(string.Empty);
                var blobDirectory = rootBlobDirectory.GetDirectoryReference(production.Id);

                if (await fileDirectory.ExistsAsync())
                {
                    await this.ArchiveTask(fileDirectory, blobDirectory, production.Id);
                }
                else
                {
                    throw new DirectoryNotFoundException($"WIP production directory {fileDirectory.Name} not found in fileshare.");
                }
            }
            catch (Exception ex)
            {
                this.log.LogError($"Archiver: Error  while archiving a production : {ex.Message}");
                await this.productionRepository.UpdateStatusOfProduction(productionId, DirectoryStatus.Archiving.ToString(), DirectoryStatus.Online.ToString());
                this.log.LogError($"Archiver: Error  while archiving a production. Production status has been updated back to Online.");
                throw;
            }

            this.log.LogInformation($"Archiver took : {DateTime.UtcNow - startTime} for {production.Name}");
        }

        private bool IsValidProduction(Production production, double duration)
        {
            DateTime currentTime = DateTime.UtcNow.AddMinutes(-duration);
            if (production.Status == DirectoryStatus.Online.ToString())
            {
                return true;
            }

            if (production.Status == DirectoryStatus.Archiving.ToString() && production.StateChangeDateTime < currentTime)
            {
                return true;
            }

            return false;
        }

        private async Task ArchiveTask(CloudFileDirectory fileDirectory, CloudBlobDirectory blobDirectory, string productionId)
        {
            this.log.LogInformation($"ArchiveTask: Entering...");
            var startTime = DateTime.UtcNow;
            await blobDirectory.Container.CreateIfNotExistsAsync();
            await this.TransferFilesAsync(fileDirectory, blobDirectory);
            await this.PreservePropertiesArchiveAsync(fileDirectory, blobDirectory, productionId);
            var archiveEndTime = DateTime.UtcNow;

            this.log.LogInformation($"Archive: SQL update started for {fileDirectory.Name}.");
            await this.UpdateLastSyncDate(fileDirectory, productionId, archiveEndTime, DirectoryStatus.Online.ToString());
            this.log.LogInformation($"Archive: SQL update completed for {fileDirectory.Name}.");

            this.log.LogInformation($"ArchiveTask took : {DateTime.UtcNow - startTime} for {fileDirectory.Name}");
        }

        private async Task UpdateLastSyncDate(CloudFileDirectory fileDirectory, string productionId, DateTime archiveDate, string status)
        {
            this.log.LogInformation($"UpdateLastSyncDate: Entering...");
            var startTime = DateTime.UtcNow;
            var production = await this.productionRepository.GetProductionById(productionId);

            production.SizeInBytes = this.cloudStorageUtility.GetFileShareByteCount(fileDirectory);
            production.LastSyncDateTime = archiveDate;
            production.Status = status;

            await this.productionRepository.UpdateProduction(production);
            this.log.LogInformation($"UpdateLastSyncDate took : {DateTime.UtcNow - startTime} for {fileDirectory.Name}");
        }

        private async Task TransferFilesAsync(CloudFileDirectory fileDirectory, CloudBlobDirectory blobDirectory)
        {
            this.log.LogInformation($"TransferFilesAsync: Entering...");
            var startTime = DateTime.UtcNow;

            TransferCheckpoint checkpoint = null;
            var context = new DirectoryTransferContext(checkpoint);

            context.ProgressHandler = new Progress<TransferStatus>((progress) =>
            {
            });

            var cancellationSource = new CancellationTokenSource();

            var options = new CopyDirectoryOptions()
            {
                Recursive = true,
            };
            context.ShouldOverwriteCallbackAsync = this.AsyncCompareWithBlob;

            this.log.LogInformation($"Starting archive operation  for files in {fileDirectory.Name}");
            await TransferManager.CopyDirectoryAsync(fileDirectory, blobDirectory, CopyMethod.ServiceSideSyncCopy, options, context, cancellationSource.Token);
            this.log.LogInformation($"Archive operation completed for files in {fileDirectory.Name}");

            if (cancellationSource.IsCancellationRequested)
            {
                    await TransferManager.CopyDirectoryAsync(fileDirectory, blobDirectory, CopyMethod.ServiceSideSyncCopy, options, context, cancellationSource.Token);
            }

            this.log.LogInformation($"TransferFilesAsync took : {DateTime.UtcNow - startTime} for {fileDirectory.Name}");
        }

        private async Task<bool> AsyncCompareWithBlob(object source, object destination)
        {
            this.log.LogInformation($"AsyncCompareWithBlob: Entering...");
            var startTime = DateTime.UtcNow;

            var sourceUri = new Uri(source.ToString());
            var sourceFile = new CloudFile(sourceUri, this.AccountStorageWip.Credentials);
            await sourceFile.FetchAttributesAsync();
            var fileDate = sourceFile.Properties.LastModified;

            var destinationUri = new Uri(destination.ToString());
            var destFile = new CloudBlob(destinationUri, this.AccountStorageArchive.Credentials);
            await destFile.FetchAttributesAsync();
            var blobDate = destFile.Properties.LastModified;

            this.log.LogInformation($"AsyncCompareWithBlob took : {DateTime.UtcNow - startTime} for {sourceUri.AbsolutePath}");
            return fileDate > blobDate;
        }

        private async Task PreservePropertiesArchiveAsync(CloudFileDirectory fileDirectory, CloudBlobDirectory blobDirectory, string productionId)
        {
            this.log.LogInformation($"PreservePropertiesArchiveAsync: Entering...");
            var startTime = DateTime.UtcNow;
            var properties = new ProductionPropertiesInfo();
            var propertiesFileName = productionId + DatamovementConstants.PropertyFileNameExtention;

            properties.Id = productionId;
            properties.Items = await this.GetSubitemProperties(fileDirectory);

            var productionProperties = new PropertiesInfo();
            await fileDirectory.FetchAttributesAsync();
            productionProperties.AbsolutePath = fileDirectory.Uri.AbsolutePath;
            productionProperties.Name = fileDirectory.Name;
            productionProperties.CreationTimeUtc = fileDirectory.Properties.CreationTime;
            productionProperties.LastWriteTimeUtc = fileDirectory.Properties.LastWriteTime;
            productionProperties.Type = ItemType.Folder.ToString();

            properties.Items.Add(productionProperties.AbsolutePath, productionProperties);

            var serializerSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateParseHandling = DateParseHandling.DateTimeOffset
            };

            string propertiesText = JsonConvert.SerializeObject(properties, serializerSettings);

            if (await fileDirectory.ExistsAsync())
            {
                var fileRef = blobDirectory.GetBlockBlobReference(propertiesFileName);
                fileRef.UploadText(propertiesText);
            }

            string nameUri = productionProperties.AbsolutePath.Split("/")[2];
            await this.DeletefromArchive(properties, blobDirectory, nameUri);
            this.log.LogInformation($"PreservePropertiesArchiveAsync took : {DateTime.UtcNow - startTime} for {fileDirectory.Name}");
        }

        private async Task<IDictionary<string, PropertiesInfo>> GetSubitemProperties(CloudFileDirectory fileDirectory)
        {
            var propertiesInfo = new Dictionary<string, PropertiesInfo>();

            foreach (IListFileItem productionDir in fileDirectory.ListFilesAndDirectories())
            {
                var itemProperty = new PropertiesInfo();
                switch (productionDir)
                {
                    case CloudFileDirectory directory:
                        await directory.FetchAttributesAsync();
                        itemProperty.AbsolutePath = directory.Uri.AbsolutePath;
                        itemProperty.Name = directory.Name;
                        itemProperty.CreationTimeUtc = directory.Properties.CreationTime;
                        itemProperty.LastWriteTimeUtc = directory.Properties.LastWriteTime;
                        itemProperty.Type = ItemType.Folder.ToString();
                        propertiesInfo.Add(itemProperty.AbsolutePath, itemProperty);
                        var result = await this.GetSubitemProperties(directory);
                        result.ToList().ForEach(x => propertiesInfo.Add(x.Key, x.Value));
                        break;

                    case CloudFile file:
                        await file.FetchAttributesAsync();
                        itemProperty.AbsolutePath = file.Uri.AbsolutePath;
                        itemProperty.Name = file.Name;
                        itemProperty.CreationTimeUtc = file.Properties.CreationTime;
                        itemProperty.LastWriteTimeUtc = file.Properties.LastWriteTime;
                        itemProperty.Type = ItemType.File.ToString();
                        propertiesInfo.Add(itemProperty.AbsolutePath, itemProperty);
                        break;
                }
            }

            return propertiesInfo;
        }

        private async Task DeletefromArchive(ProductionPropertiesInfo propertiesInfo, CloudBlobDirectory blobDirectory, string productionName)
        {
            this.log.LogInformation($"Starting compare operation to check if any files are deleted in WIP production");
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var blobList = await blobDirectory.ListBlobsSegmentedAsync(blobContinuationToken);
                blobContinuationToken = blobList.ContinuationToken;
                if (blobList.Results.Any())
                {
                    foreach (IListBlobItem blob in blobList.Results)
                    {
                        var blobType = blob.GetType();
                        if (blobType == typeof(CloudBlobDirectory))
                        {
                            var subfolderDirectory = (CloudBlobDirectory)blob;
                            await this.DeletefromArchive(propertiesInfo, subfolderDirectory, productionName);
                        }
                        else
                        {
                            var blobPath = blob.Uri.AbsolutePath;
                            if (blobPath.EndsWith(".metadata"))
                            {
                                continue;
                            }

                            var pathArray = blobPath.Split("/");

                            var wipPath = blobPath.Replace(pathArray[2], productionName);
                            if (!propertiesInfo.Items.ContainsKey(wipPath) && blobType == typeof(CloudBlockBlob))
                            {
                                await ((CloudBlockBlob)blob).DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, null, null);
                                this.log.LogInformation($"Found file to delete: {wipPath} ");
                            }
                        }
                    }
                }
            }
            while (blobContinuationToken != null);
        }
    }
}
