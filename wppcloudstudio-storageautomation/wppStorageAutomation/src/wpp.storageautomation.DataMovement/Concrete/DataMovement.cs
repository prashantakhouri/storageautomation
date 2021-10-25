// <copyright file="DataMovement.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.Azure.Storage.File;
using Microsoft.Extensions.Logging;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.DataMovement.Contracts;
using Wpp.StorageAutomation.DataMovement.Models;
using Wpp.StorageAutomation.DataMovement.Repository;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation.DataMovement
{
    /// <summary>
    ///   The data movement.
    /// </summary>
    public class DataMovement : IDataMovement
    {
        private readonly ICloudStorageUtility cloudStorageUtility;
        private readonly IDirectoryHelper directoryHelper;
        private readonly IStorageAccountConfig storageAccountConfig;
        private readonly IProductionRepository productionRepository;
        private readonly IProductionStoreRepository productionStoreRepository;
        private readonly IActiveDirectoryUtility activeDirectoryUtility;
        private readonly ISddlBuilderUtility sddlBuilderUtility;
        private readonly ILogger<DataMovement> log;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMovement" /> class.
        /// </summary>
        /// <param name="storageAccountConfig">The storage account configuration.</param>
        /// <param name="cloudStorageUtility">The cloud storage utility.</param>
        /// <param name="activeDirectoryUtility">The active directory utility.</param>
        /// <param name="directoryHelper">The directory helper.</param>
        /// <param name="productionRepository">The production repository.</param>
        /// <param name="productionStoreRepository">The production store repository.</param>
        /// <param name="sddlBuilderUtility">The SDDL builder utility.</param>
        /// <param name="log">The log.</param>
        public DataMovement(
            IStorageAccountConfig storageAccountConfig,
            ICloudStorageUtility cloudStorageUtility,
            IActiveDirectoryUtility activeDirectoryUtility,
            IDirectoryHelper directoryHelper,
            IProductionRepository productionRepository,
            IProductionStoreRepository productionStoreRepository,
            ISddlBuilderUtility sddlBuilderUtility,
            ILogger<DataMovement> log)
        {
            this.storageAccountConfig = storageAccountConfig;
            this.cloudStorageUtility = cloudStorageUtility;
            this.activeDirectoryUtility = activeDirectoryUtility;
            this.directoryHelper = directoryHelper;
            this.productionRepository = productionRepository;
            this.productionStoreRepository = productionStoreRepository;
            this.sddlBuilderUtility = sddlBuilderUtility;
            this.log = log;
        }

        /// <inheritdoc/>
        public async Task<ArchiveAllResponse> ArchiveAsync()
        {
            var response = new ArchiveAllResponse();
            var archiveTasks = new List<Task<KeyValuePair<string, DateTime>>>();
            var archiveDate = DateTime.UtcNow;

            var prodStores = await this.productionStoreRepository.GetAllProductionStores();

            await this.directoryHelper.SetProductionStoreInfoAsync();

            foreach (var prodStore in prodStores)
            {
                if (prodStore.WipkeyName == null || prodStore.WipkeyName == string.Empty || prodStore.ArchiveKeyName == null || prodStore.ArchiveKeyName == string.Empty)
                {
                    throw new KeyNotFoundException($"Storage key for production store not found: {prodStore.Name}");
                }

                CloudStorageAccount accountStorageWip = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(prodStore.WipkeyName));
                CloudStorageAccount accountStorageArchive = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(prodStore.ArchiveKeyName));
                var productions = await this.productionRepository.GetOnlineProductionsByProdStoreId(prodStore.Id);

                var fileShare = this.cloudStorageUtility.GetCloudFileShare(accountStorageWip, prodStore.Name);
                var blobContainer = this.cloudStorageUtility.GetBlobContainer(accountStorageArchive, prodStore.Name);

                foreach (var production in productions)
                {
                    await this.productionRepository.UpdateStatusOfProduction(production.Id, DirectoryStatus.Online.ToString(), DirectoryStatus.Archiving.ToString());
                    var fileDirectory = fileShare.GetDirectoryReference(production.Name);
                    var blobDirectory = blobContainer.GetDirectoryReference(production.Id);
                    try
                    {
                        if (await fileDirectory.ExistsAsync())
                        {
                            archiveTasks.Add(this.ArchiveTask(fileDirectory, blobDirectory, production.Id));
                        }
                        else
                        {
                            throw new DirectoryNotFoundException($"WIP production directory {fileDirectory.Name} not found in fileshare.");
                        }
                    }
                    catch (TransferException ex)
                    {
                        if (ex.Message.Contains("A transfer operation with the same source and destination already exists."))
                        {
                            this.log.LogInformation(ex.Message);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (Exception)
                    {
                        await this.productionRepository.UpdateStatus(DirectoryStatus.Archiving.ToString(), DirectoryStatus.Online.ToString());
                        throw;
                    }
                }
            }

            try
            {
                await Task.WhenAll(archiveTasks);
            }
            catch (Exception)
            {
                await this.productionRepository.UpdateStatus(DirectoryStatus.Archiving.ToString(), DirectoryStatus.Online.ToString());
                throw;
            }

            response.Message = $"Archived successfully";
            response.ArchiveDate = archiveDate;
            return response;
        }

        /// <inheritdoc/>
        public async Task<ArchiveProductionStoreResponse> ArchiveProductionStoreAsync(string productionStoreId)
        {
            var response = new ArchiveProductionStoreResponse();
            var archiveTasks = new List<Task<KeyValuePair<string, DateTime>>>();
            var archiveDate = DateTime.UtcNow;

            var prodStore = await this.productionStoreRepository.GetProductionStoreById(productionStoreId);
            if (prodStore == null)
            {
                throw new DirectoryNotFoundException($"Production Store Id : {productionStoreId} entry was not found in production store table.");
            }

            await this.directoryHelper.SetProductionStoreInfoAsync();

            CloudStorageAccount accountStorageWip = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(prodStore.WipkeyName));
            CloudStorageAccount accountStorageArchive = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(prodStore.ArchiveKeyName));

            var fileClient = this.cloudStorageUtility.GetFileClient(accountStorageWip);
            var fileShare = fileClient.GetShareReference(prodStore.Name);

            if (!await fileShare.ExistsAsync())
            {
                throw new DirectoryNotFoundException($"Production Store Name : {prodStore.Name} does not exist in WIP.");
            }

            var productions = await this.productionRepository.GetOnlineProductionsByProdStoreId(productionStoreId);
            var fileShareRoot = fileShare.GetRootDirectoryReference();
            var blobContainer = this.cloudStorageUtility.GetBlobContainer(accountStorageArchive, prodStore.Name);
            foreach (var production in productions)
            {
                await this.productionRepository.UpdateStatusOfProduction(production.Id, DirectoryStatus.Online.ToString(), DirectoryStatus.Archiving.ToString());

                try
                {
                    var fileDirectory = fileShareRoot.GetDirectoryReference(production.Name);
                    var blobDirectory = blobContainer.GetDirectoryReference(production.Id);

                    if (await fileDirectory.ExistsAsync())
                    {
                        archiveTasks.Add(this.ArchiveTask(fileDirectory, blobDirectory, production.Id));
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"WIP production directory {fileDirectory.Name} not found in fileshare.");
                    }
                }
                catch (TransferException ex)
                {
                    if (ex.Message.Contains("A transfer operation with the same source and destination already exists."))
                    {
                        this.log.LogInformation(ex.Message);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    await this.productionRepository.UpdateStatusOfProductionStore(productionStoreId, DirectoryStatus.Archiving.ToString(), DirectoryStatus.Online.ToString());
                    throw;
                }
            }

            try
            {
                await Task.WhenAll(archiveTasks);
            }
            catch (Exception)
            {
                await this.productionRepository.UpdateStatusOfProductionStore(productionStoreId, DirectoryStatus.Archiving.ToString(), DirectoryStatus.Online.ToString());
                throw;
            }

            response.ArchiveDate = archiveDate;
            response.Message = $"Archived {productionStoreId} successfully";
            response.Name = prodStore.Name;
            return response;
        }

        /// <inheritdoc/>
        public async Task<ArchiveProductionResponse> ArchiveProductionAsync(ProductionRequest archiveProductionRequest)
        {
            var archiveTime = default(KeyValuePair<string, DateTime>);
            var production = await this.productionRepository.GetProductionById(archiveProductionRequest.ProductionId);
            if (production == null || production.ProductionStoreId != archiveProductionRequest.ProductionStoreId)
            {
                throw new DirectoryNotFoundException($"Production Store Id : {archiveProductionRequest.ProductionStoreId}, Production Id : {archiveProductionRequest.ProductionId} entry was not found in production table.");
            }

            var prodStore = await this.productionStoreRepository.GetProductionStoreById(archiveProductionRequest.ProductionStoreId);
            if (prodStore == null)
            {
                throw new DirectoryNotFoundException($"Production Store Id : {archiveProductionRequest.ProductionStoreId} entry was not found in production store table.");
            }

            if (production.Status != DirectoryStatus.Online.ToString())
            {
                throw new DirectoryNotFoundException($"Production Id : {archiveProductionRequest.ProductionId} is not Online.");
            }

            await this.directoryHelper.SetProductionStoreInfoAsync();

            CloudStorageAccount accountStorageWip = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(prodStore.WipkeyName));
            CloudStorageAccount accountStorageArchive = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(prodStore.ArchiveKeyName));

            await this.productionRepository.UpdateStatusOfProduction(archiveProductionRequest.ProductionId, DirectoryStatus.Online.ToString(), DirectoryStatus.Archiving.ToString());

            try
            {
                var fileDirectory = this.cloudStorageUtility.GetCloudFileDirectory(accountStorageWip, prodStore.Name, production.Name);
                var blobDirectory = this.cloudStorageUtility.GetBlobDirectory(accountStorageArchive, prodStore.Name, production.Id);

                if (await fileDirectory.ExistsAsync())
                {
                    archiveTime = await this.ArchiveTask(fileDirectory, blobDirectory, production.Id);
                }
                else
                {
                    throw new DirectoryNotFoundException($"WIP production directory {fileDirectory.Name} not found in fileshare.");
                }

                this.log.LogInformation($"Archive: SQL update started for {production.Name}.");
                await this.UpdateLastSyncDate(archiveTime.Key, archiveTime.Value, DirectoryStatus.Online.ToString());
                this.log.LogInformation($"Archive: SQL update completed for {production.Name}.");

                var response = new ArchiveProductionResponse()
                {
                    Id = production.Id,
                    Name = production.Name,
                    Uri = production.Wipurl,
                    Status = production.Status,
                    CreatedDate = production.CreatedDateTime,
                    LastSyncDateTime = production.LastSyncDateTime,
                    SizeInBytes = production.SizeInBytes
                };

                return response;
            }
            catch (Exception)
            {
                await this.productionRepository.UpdateStatusOfProduction(archiveProductionRequest.ProductionId, DirectoryStatus.Archiving.ToString(), DirectoryStatus.Online.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ProductionResponse> RestoreProductionAsync(ProductionRequest request)
        {
            this.log.LogInformation($"Restore validations started.");
            var prodStore = await this.productionStoreRepository.GetProductionStoreById(request.ProductionStoreId);
            if (prodStore == null)
            {
                throw new DirectoryNotFoundException($"Production Store Id : {request.ProductionStoreId} entry was not found in production store table.");
            }

            var wipConnectionString = this.storageAccountConfig.GetStorageConnectionString(prodStore.WipkeyName);
            CloudStorageAccount accountStorageWip = CloudStorageAccount.Parse(wipConnectionString);
            CloudStorageAccount accountStorageArchive = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(prodStore.ArchiveKeyName));

            if (!await this.cloudStorageUtility.FileShareExists(accountStorageWip, prodStore.Name))
            {
                throw new DirectoryNotFoundException($"Production Store Name : {prodStore.Name} does not exist in WIP.");
            }

            var production = await this.productionRepository.GetProductionById(request.ProductionId);
            if (production == null || production.ProductionStoreId != request.ProductionStoreId)
            {
                throw new DirectoryNotFoundException($"Production store : {request.ProductionStoreId} Production: {request.ProductionId} entry was not found in production table.");
            }

            if (production.Status == DirectoryStatus.Online.ToString())
            {
                throw new ArgumentException($"Production: {production.Name} status is online");
            }

            if (production.DeletedFlag == true)
            {
                throw new ArgumentException($"Production: {production.Name} cannot be restored since it was deleted.");
            }

            var fileDirectory = this.cloudStorageUtility.GetCloudFileDirectory(accountStorageWip, prodStore.Name, production.Name);
            if (await fileDirectory.ExistsAsync())
            {
                throw new ArgumentException($"Production: {production.Name} already exists in WIP");
            }

            var blobDirectory = this.cloudStorageUtility.GetBlobDirectory(accountStorageArchive, prodStore.Name, production.Id);
            if (!blobDirectory.Container.Exists() || !blobDirectory.ListBlobs().Any())
            {
                throw new DirectoryNotFoundException($"Production store : {prodStore.Name} or Production with Id : {production.Id} does not exist in Archive");
            }

            this.log.LogInformation($"Restore validation completed.");

            try
            {
                await this.productionRepository.UpdateStatusOfProduction(production.Id, DirectoryStatus.Offline.ToString(), DirectoryStatus.MakingOnline.ToString());
                this.log.LogInformation($"Restore: Status updated. Data movement library operations starting.");
                await this.directoryHelper.TransferFilesAsync(fileDirectory, blobDirectory, DataMovementType.Restore);
            }
            catch (Exception)
            {
                await fileDirectory.DeleteIfExistsAsync();
                await this.productionRepository.UpdateStatusOfProduction(production.Id, DirectoryStatus.MakingOnline.ToString(), DirectoryStatus.Offline.ToString());
                throw;
            }

            try
            {
                this.log.LogInformation($"Restore properties started for {production.Name}.");
                await this.directoryHelper.PreservePropertiesRestoreAsync(fileDirectory, blobDirectory, production.Id, prodStore);
                this.log.LogInformation($"Restore properties completed for {production.Name}.");
            }
            catch (Exception)
            {
                await this.productionRepository.UpdateStatusOfProduction(production.Id, DirectoryStatus.MakingOnline.ToString(), DirectoryStatus.Online.ToString());
                throw;
            }

            this.log.LogInformation($"Setting ACL for restore at root level.");
            fileDirectory.FilePermission = this.sddlBuilderUtility.BuildSDDL(prodStore, ActivityType.Create);
            await fileDirectory.SetPropertiesAsync();
            this.log.LogInformation($"ACL for restore at root level completed.");
            this.log.LogInformation($"Restore: SQL update started for {production.Name}.");
            var response = await this.CreateRestoreProductionResponse(production.Id);
            this.log.LogInformation($"Restore: SQL update completed for {production.Name}.");
            return response;
        }

        /// <inheritdoc/>
        public async Task<ProductionResponse> MakeProductionOfflineAsync(ProductionRequest request)
        {
            var prodStore = await this.productionStoreRepository.GetProductionStoreById(request.ProductionStoreId);
            if (prodStore == null)
            {
                throw new DirectoryNotFoundException($"Production Store Id : {request.ProductionStoreId} entry was not found in production store table.");
            }

            var production = await this.productionRepository.GetProductionById(request.ProductionId);
            if (production == null || production.ProductionStoreId != request.ProductionStoreId)
            {
                throw new DirectoryNotFoundException($"Production store : {request.ProductionStoreId} Production: {request.ProductionId} entry was not found in production table.");
            }

            if (production.Status == DirectoryStatus.Offline.ToString())
            {
                throw new DirectoryNotFoundException($"Production Id : {request.ProductionId} is already offline.");
            }

            await this.directoryHelper.SetProductionStoreInfoAsync();

            CloudStorageAccount accountStorageWip = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(prodStore.WipkeyName));
            CloudStorageAccount accountStorageArchive = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(prodStore.ArchiveKeyName));
            if (!await this.cloudStorageUtility.FileShareExists(accountStorageWip, prodStore.Name))
            {
                throw new DirectoryNotFoundException($"Production Store Name : {prodStore.Name} does not exist in WIP.");
            }

            var fileDirectory = this.cloudStorageUtility.GetCloudFileDirectory(accountStorageWip, prodStore.Name, production.Name);
            var blobDirectory = this.cloudStorageUtility.GetBlobDirectory(accountStorageArchive, prodStore.Name, production.Id);

            if (!await fileDirectory.ExistsAsync())
            {
                throw new ArgumentException($"Production: {production.Name} does not exist in WIP.");
            }

            this.log.LogInformation($"Check open file handles starting.");
            FileContinuationToken continuationToken = new FileContinuationToken();
            do
            {
                var openFileHandles = await fileDirectory.ListHandlesSegmentedAsync(continuationToken, null, true);
                continuationToken = openFileHandles.ContinuationToken;
                int openFileCount = openFileHandles.Results.Count<FileHandle>();
                if (openFileCount > 0)
                {
                    throw new InvalidOperationException($"Offline operation cannot be performed with open files or folders. Please close all files and folders and try again. ");
                }
            }
            while (continuationToken.NextMarker != null);
            this.log.LogInformation($"Check open file handles completed.");

            this.log.LogInformation($"Set readonly permission starting.");
            fileDirectory.FilePermission = this.storageAccountConfig.WPPReadonlySDDLConfig;
            await fileDirectory.SetPropertiesAsync();
            this.log.LogInformation($"Set readonly permission completed.");

            var archiveTime = default(KeyValuePair<string, DateTime>);
            await this.productionRepository.UpdateStatusOfProduction(production.Id, DirectoryStatus.Online.ToString(), DirectoryStatus.MakingOffline.ToString());
            try
            {
                archiveTime = await this.ArchiveTask(fileDirectory, blobDirectory, production.Id);

                this.log.LogInformation($"Make offline: SQL update starting.");
                await this.UpdateLastSyncDate(archiveTime.Key, archiveTime.Value, DirectoryStatus.MakingOffline.ToString());
                this.log.LogInformation($"Make offline: SQL update completed.");

                this.log.LogInformation($"Make offline: Delete folder starting.");
                await this.cloudStorageUtility.DeleteFileShareDirectory(fileDirectory);
                this.log.LogInformation($"Make offline: Delete folder completed.");

                this.log.LogInformation($"Make offline: SQL update starting.");
                production = await this.productionRepository.GetProductionById(request.ProductionId);
                production.Status = DirectoryStatus.Offline.ToString();
                production.ModifiedDateTime = DateTime.UtcNow;
                await this.productionRepository.UpdateProduction(production);
                this.log.LogInformation($"Make offline: SQL update completed.");
            }
            catch (Exception)
            {
                await this.productionRepository.UpdateStatusOfProduction(production.Id, DirectoryStatus.MakingOffline.ToString(), DirectoryStatus.Online.ToString());
                throw;
            }

            var response = new ProductionResponse()
            {
                Id = production.Id,
                Name = production.Name,
                Uri = production.Wipurl,
                Status = production.Status,
                CreatedDate = production.CreatedDateTime,
                ModifiedDate = production.ModifiedDateTime,
                SizeInBytes = production.SizeInBytes
            };

            return response;
        }

        private async Task<KeyValuePair<string, DateTime>> ArchiveTask(CloudFileDirectory fileDirectory, CloudBlobDirectory blobDirectory, string productionId)
        {
            await blobDirectory.Container.CreateIfNotExistsAsync();
            await this.directoryHelper.TransferFilesAsync(fileDirectory, blobDirectory, DataMovementType.Archive);
            await this.directoryHelper.PreservePropertiesArchiveAsync(fileDirectory, blobDirectory, productionId);
            var archiveEndTime = DateTime.UtcNow;

            this.log.LogInformation($"Archive: SQL update starting.");
            await this.UpdateLastSyncDate(productionId, archiveEndTime, DirectoryStatus.Online.ToString());
            this.log.LogInformation($"Archive: SQL update completed.");

            return new KeyValuePair<string, DateTime>(productionId, archiveEndTime);
        }

        private async Task UpdateLastSyncDate(string productionId, DateTime archiveDate, string status)
        {
            var production = await this.productionRepository.GetProductionById(productionId);
            var prodStore = await this.productionStoreRepository.GetProductionStoreById(production.ProductionStoreId);

            CloudStorageAccount accountStorageWip = CloudStorageAccount.Parse(this.storageAccountConfig.GetStorageConnectionString(prodStore.WipkeyName));

            var fileDirectory = this.cloudStorageUtility.GetCloudFileDirectory(accountStorageWip, prodStore.Name, production.Name);
            production.SizeInBytes = this.cloudStorageUtility.GetFileShareByteCount(fileDirectory);
            production.LastSyncDateTime = archiveDate;
            production.Status = status;

            await this.productionRepository.UpdateProduction(production);
        }

        private async Task<ProductionResponse> CreateRestoreProductionResponse(string productionId)
        {
            var production = await this.productionRepository.GetProductionById(productionId);

            if (production != null)
            {
                production.Status = DirectoryStatus.Online.ToString();
                production.ModifiedDateTime = DateTime.UtcNow;
                await this.productionRepository.UpdateProduction(production);
            }

            ProductionResponse res = new ProductionResponse()
            {
                Id = production?.Id,
                Name = production?.Name,
                Uri = production?.Wipurl,
                Status = production?.Status,
                CreatedDate = production?.CreatedDateTime,
                ModifiedDate = production?.ModifiedDateTime,
                SizeInBytes = production?.SizeInBytes
            };

            return res;
        }
    }
}