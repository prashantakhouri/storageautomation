// <copyright file="DirectoryHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.Azure.Storage.File;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Common.Constants;
using Wpp.StorageAutomation.DataMovement.Contracts;
using Wpp.StorageAutomation.DataMovement.Models;
using Wpp.StorageAutomation.DataMovement.Repository;
using Wpp.StorageAutomation.Entities.Models;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation.DataMovement
{
    /// <summary>
    ///   The directory helper.
    /// </summary>
    public class DirectoryHelper : IDirectoryHelper
    {
        private readonly ICloudStorageUtility cloudStorageUtility;
        private readonly IProductionRepository productionRepository;
        private readonly IProductionStoreRepository productionStoreRepository;
        private readonly ILogger<DirectoryHelper> log;
        private readonly IStorageAccountConfig storageAccountConfig;
        private readonly ISddlBuilderUtility sddlBuilderUtility;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryHelper" /> class.
        /// </summary>
        /// <param name="storageAccountConfig">The storage account configuration.</param>
        /// <param name="cloudStorageUtility">The cloud storage utility.</param>
        /// <param name="productionRepository">The production repository.</param>
        /// <param name="productionStoreRepository">The production store repository.</param>
        /// <param name="sddlBuilderUtility">The sddlBuilderUtility.</param>
        /// <param name="log">The log.</param>
        public DirectoryHelper(
            IStorageAccountConfig storageAccountConfig,
            ICloudStorageUtility cloudStorageUtility,
            IProductionRepository productionRepository,
            IProductionStoreRepository productionStoreRepository,
            ILogger<DirectoryHelper> log,
            ISddlBuilderUtility sddlBuilderUtility)
        {
            this.cloudStorageUtility = cloudStorageUtility;
            this.productionRepository = productionRepository;
            this.productionStoreRepository = productionStoreRepository;
            this.log = log;
            this.storageAccountConfig = storageAccountConfig;
            this.sddlBuilderUtility = sddlBuilderUtility;
        }

        private static IDictionary<string, KeyValuePair<string, string>> ProductionStoresInfo { get; set; }

        /// <inheritdoc/>
        public async Task TransferFilesAsync(CloudFileDirectory fileDirectory, CloudBlobDirectory blobDirectory, DataMovementType dataMovementType)
        {
            TransferCheckpoint checkpoint = null;
            var context = this.cloudStorageUtility.GetDirectoryTransferContext(checkpoint);
            var cancellationSource = new CancellationTokenSource();

            var options = new CopyDirectoryOptions()
            {
                Recursive = true,
            };

            if (dataMovementType == DataMovementType.Archive)
            {
                context.ShouldOverwriteCallbackAsync = this.AsyncCompareWithBlob;

                this.log.LogInformation($"Starting archive operation  for files in {fileDirectory.Name}");
                await TransferManager.CopyDirectoryAsync(fileDirectory, blobDirectory, CopyMethod.ServiceSideSyncCopy, options, context, cancellationSource.Token);
                this.log.LogInformation($"Archive operation completed for files in {fileDirectory.Name}");
            }
            else if (dataMovementType == DataMovementType.Restore)
            {
                this.log.LogInformation($"Starting restore operation for files to {fileDirectory.Name}");
                await TransferManager.CopyDirectoryAsync(blobDirectory, fileDirectory, CopyMethod.ServiceSideSyncCopy, options, context, cancellationSource.Token);
                this.log.LogInformation($"Restore operation for files to {fileDirectory.Name} completed");
            }

            if (cancellationSource.IsCancellationRequested)
            {
                checkpoint = context.LastCheckpoint;
                context = this.cloudStorageUtility.GetDirectoryTransferContext(checkpoint);
                if (dataMovementType == DataMovementType.Archive)
                {
                    await TransferManager.CopyDirectoryAsync(fileDirectory, blobDirectory, CopyMethod.ServiceSideSyncCopy, options, context, cancellationSource.Token);
                }
                else if (dataMovementType == DataMovementType.Restore)
                {
                    await TransferManager.CopyDirectoryAsync(blobDirectory, fileDirectory, CopyMethod.ServiceSideSyncCopy, options, context, cancellationSource.Token);
                }
            }
        }

        /// <inheritdoc/>
        public async Task PreservePropertiesArchiveAsync(CloudFileDirectory fileDirectory, CloudBlobDirectory blobDirectory, string productionId)
        {
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
        }

        /// <inheritdoc/>
        public async Task PreservePropertiesRestoreAsync(CloudFileDirectory fileDirectory, CloudBlobDirectory blobDirectory, string productionId, ProductionStore productionStore)
        {
            Console.WriteLine($"Preserve op for restore started for {fileDirectory.Name} at {DateTime.Now}");
            var contents = string.Empty;

            var propertiesFileName = productionId + DatamovementConstants.PropertyFileNameExtention;

            var propertyFile = blobDirectory.GetBlockBlobReference(propertiesFileName);
            if (await propertyFile.ExistsAsync())
            {
                contents = await propertyFile.DownloadTextAsync();
            }
            else
            {
                throw new KeyNotFoundException($"All files restored but the file ' {propertiesFileName} ' was not found in Archive. As a result, created date and updated date didn't get preserved. Empty directories are lost.");
            }

            var serializerSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateParseHandling = DateParseHandling.DateTimeOffset
            };

            ProductionPropertiesInfo prodPropertiesInfo = JsonConvert.DeserializeObject<ProductionPropertiesInfo>(contents, serializerSettings);

            await fileDirectory.GetFileReference(propertiesFileName).DeleteIfExistsAsync();

            string sddl = this.sddlBuilderUtility.BuildSDDL(productionStore, ActivityType.Restore);

            foreach (var item in prodPropertiesInfo.Items)
            {
                var absolutePath = HttpUtility.UrlDecode(item.Value.AbsolutePath);
                var pathElements = new List<string>(absolutePath.Split("/"));

                pathElements.Remove(pathElements.FirstOrDefault());
                pathElements.Remove(pathElements.FirstOrDefault());
                pathElements.Remove(pathElements.FirstOrDefault());

                var path = string.Join("/", pathElements);

                if (item.Value.Type == ItemType.File.ToString())
                {
                    var file = item.Value;
                    var fileRef = fileDirectory.GetFileReference(path);

                    if (await fileRef.ExistsAsync())
                    {
                        await fileRef.FetchAttributesAsync();
                        fileRef.Properties.CreationTime = file.CreationTimeUtc;
                        fileRef.Properties.LastWriteTime = file.LastWriteTimeUtc;
                        fileRef.FilePermission = sddl;
                        await fileRef.SetPropertiesAsync();
                    }
                }
                else if (item.Value.Type == ItemType.Folder.ToString())
                {
                    var folder = item.Value;
                    CloudFileDirectory folderRef;

                    if (path == string.Empty)
                    {
                        folderRef = fileDirectory;
                    }
                    else
                    {
                        folderRef = fileDirectory.GetDirectoryReference(path);
                    }

                    await folderRef.CreateIfNotExistsAsync();

                    if (await folderRef.ExistsAsync())
                    {
                        await folderRef.FetchAttributesAsync();
                        folderRef.Properties.CreationTime = folder.CreationTimeUtc;
                        folderRef.Properties.LastWriteTime = folder.LastWriteTimeUtc;
                        folderRef.FilePermission = sddl;
                        await folderRef.SetPropertiesAsync();
                    }
                }
            }

            Console.WriteLine($"Preserve op for restore ended for {fileDirectory.Name} at {DateTime.Now}");
        }

        /// <inheritdoc/>
        public async Task DeleteFromFileShareDirectory(CloudFileDirectory fileDirectory)
        {
            await this.cloudStorageUtility.DeleteFileShareDirectory(fileDirectory);
        }

        /// <inheritdoc/>
        public async Task SetProductionStoreInfoAsync()
        {
            ProductionStoresInfo = new Dictionary<string, KeyValuePair<string, string>>();
            var prodStores = await this.productionStoreRepository.GetAllProductionStores();
            foreach (var prodStore in prodStores)
            {
                ProductionStoresInfo[prodStore.Name] = new KeyValuePair<string, string>(prodStore.WipkeyName, prodStore.ArchiveKeyName);
            }
        }

        private async Task<bool> AsyncCompareWithBlob(object source, object destination)
        {
            var startTime = DateTime.UtcNow;

            var sourceUri = new Uri(source.ToString());
            var prodStoreName = sourceUri.LocalPath.Split("/")[1];

            if (!ProductionStoresInfo.ContainsKey(prodStoreName))
            {
                await this.SetProductionStoreInfoAsync();
            }

            CloudStorageAccount accountStorageWip = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(ProductionStoresInfo[prodStoreName].Key));
            CloudStorageAccount accountStorageArchive = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(ProductionStoresInfo[prodStoreName].Value));

            var sourceFile = new CloudFile(sourceUri, accountStorageWip.Credentials);
            await sourceFile.FetchAttributesAsync();
            var fileDate = sourceFile.Properties.LastModified;

            var destinationUri = new Uri(destination.ToString());
            var destFile = new CloudBlob(destinationUri, accountStorageArchive.Credentials);

            await destFile.FetchAttributesAsync();
            var blobDate = destFile.Properties.LastModified;

            this.log.LogInformation($"Async compare took : {DateTime.UtcNow - startTime} for {sourceUri.AbsolutePath}");
            return fileDate > blobDate;
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
    }
}