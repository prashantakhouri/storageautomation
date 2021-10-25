// <copyright file="Productions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using ExtensionMethod;
using Microsoft.Extensions.Logging;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Entities.Models;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation.Production
{
    /// <summary>
    /// Production.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.IProduction" />
    public class Productions : IProduction
    {
        private readonly IStorageAccountConfig storageAccountConfig;
        private readonly IProductionRepository productionRepository;
        private readonly IProductionStoreRepository productionStoreRepository;
        private readonly IActiveDirectoryUtility activeDirectoryUtility;
        private readonly ISddlBuilderUtility sddlBuilderUtility;
        private readonly ILogger<Productions> log;

        /// <summary>
        /// Initializes a new instance of the <see cref="Productions" /> class.
        /// Productions constructor.
        /// </summary>
        /// <param name="storageAccountConfig">The storage account configuration.</param>
        /// <param name="productionRepository">The production repository.</param>
        /// <param name="productionStoreRepository">The production store repository.</param>
        /// <param name="activeDirectoryUtility">The active directory utility.</param>
        /// <param name="sddlBuilderUtility">The SDDL builder utility.</param>
        /// <param name="log">The log.</param>
        public Productions(
            IStorageAccountConfig storageAccountConfig,
            IProductionRepository productionRepository,
            IProductionStoreRepository productionStoreRepository,
            IActiveDirectoryUtility activeDirectoryUtility,
            ISddlBuilderUtility sddlBuilderUtility,
            ILogger<Productions> log)
        {
            this.storageAccountConfig = storageAccountConfig;
            this.productionRepository = productionRepository;
            this.productionStoreRepository = productionStoreRepository;
            this.activeDirectoryUtility = activeDirectoryUtility;
            this.sddlBuilderUtility = sddlBuilderUtility;
            this.log = log;
        }

        /// <summary>
        /// Gets the production list.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="userGroups">The groups.</param>
        /// <returns>ProductionListResponse.</returns>
        public async Task<ProductionListResponse> GetProductionsByProductionStoreAsync(string productionStoreId, List<string> userGroups)
        {
            var prodStore = await this.productionStoreRepository.GetProductionStoreById(productionStoreId);

            if (prodStore == null)
            {
                throw new KeyNotFoundException($"Production store not found. Production Store Id : {productionStoreId}");
            }

            var productions = await this.productionRepository.GetProductionByProductionStoreId(productionStoreId);
            var productionListResponse = new ProductionListResponse()
            {
                ProductionList = productions.Select(x => new ProductionRow()
                {
                    Id = x.Id,
                    ProductionStoreId = x.ProductionStoreId,
                    Name = x.Name,
                    Uri = x.Wipurl,
                    Status = x.Status,
                    CreatedDateTime = x.CreatedDateTime,
                    LastSyncDateTime = x.LastSyncDateTime,
                    ModifiedDateTime = x.ModifiedDateTime,
                    SizeInBytes = x.SizeInBytes ?? 0,
                    IsManager = IsStoreManager(userGroups, prodStore.ManagerRoleGroupNames)
                }),
                ProductionStore = new ProductionStoreRow()
                {
                    Id = prodStore.Id,
                    Name = prodStore.Name,
                    Region = prodStore.Region,
                    Wipurl = prodStore.Wipurl,
                    WipallocatedSize = prodStore.WipallocatedSize,
                    ArchiveUrl = prodStore.ArchiveUrl,
                    ArchiveAllocatedSize = prodStore.ArchiveAllocatedSize,
                    ScaleDownTime = prodStore.ScaleDownTime,
                    ScaleUpTimeInterval = prodStore.ScaleUpTimeInterval,
                    MinimumFreeSize = prodStore.MinimumFreeSize,
                    MinimumFreeSpace = prodStore.MinimumFreeSpace,
                    OfflineTime = prodStore.OfflineTime,
                    OnlineTime = prodStore.OnlineTime,
                    ProductionOfflineTimeInterval = prodStore.ProductionOfflineTimeInterval,
                    ManagerRoleGroupNames = prodStore.ManagerRoleGroupNames,
                    UserRoleGroupNames = prodStore.UserRoleGroupNames,
                    WipkeyName = prodStore.WipkeyName,
                    ArchiveKeyName = prodStore.ArchiveKeyName
                }
            };
            return productionListResponse;
        }

        /// <summary>
        /// Creates the production.
        /// </summary>
        /// <param name="productionReq">The production req.</param>
        /// <returns>
        ///   The production response.
        /// </returns>
        public async Task<ProductionResponse> CreateProduction(ProductionRequest productionReq)
        {
            if (productionReq == null || !productionReq.DirectoryTree.Any())
            {
                return null;
            }

            return await this.CreateDirectoryTree(productionReq);
        }

        /// <summary>
        /// Creates the folder structure for a prouction.
        /// </summary>
        /// <param name="productionReq">The production request.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task<ProductionResponse> CreateDirectoryTree(ProductionRequest productionReq)
        {
            string parentDirName = string.Empty;
            string parentDirUri = string.Empty;
            ProductionResponse resObj = null;

            var prodStore = await this.productionStoreRepository.GetProductionStoreById(productionReq.ProductionStoreId);
            if (prodStore == null)
            {
                throw new KeyNotFoundException($"Production store not found. Production store ID: {productionReq.ProductionStoreId}");
            }

            if (prodStore.WipkeyName == null || prodStore.WipkeyName == string.Empty)
            {
                throw new KeyNotFoundException($"Storage key for production store not found: {prodStore.Name}");
            }

            var productionExists = await this.productionRepository.ProductionExists(productionReq.Tokens.FirstOrDefault().ProductionToken.ToString().Trim());
            if (productionExists)
            {
                throw new DuplicateNameException($"Duplicate name not allowed: {productionReq.Tokens.FirstOrDefault().ProductionToken.ToString().Trim()}");
            }

            string wipConnectionString = this.storageAccountConfig.GetStorageConnectionString(prodStore.WipkeyName);
            ShareClient fileShareDirectory = new ShareClient(wipConnectionString, prodStore.Name);

            if (!await fileShareDirectory.ExistsAsync())
            {
                throw new KeyNotFoundException($"Production store not found. Production store ID: {productionReq.ProductionStoreId}");
            }

            foreach (var dir in productionReq.DirectoryTree)
            {
                // Extracting productionName from the Tokens
                parentDirName = productionReq.Tokens.FirstOrDefault().ProductionToken.ToString().Trim();
                ShareDirectoryClient rootDirectory = fileShareDirectory.GetDirectoryClient(parentDirName);

                if (await rootDirectory.ExistsAsync())
                {
                    throw new DuplicateNameException($"Duplicate name not allowed: {parentDirName}");
                }

                this.CheckDuplicateDirectoryName(productionReq);

                ShareDirectoryClient parentDirectory = fileShareDirectory.GetDirectoryClient(parentDirName);
                parentDirUri = parentDirectory.Uri.AbsolutePath.ToString();
                resObj = this.CreateProductionResponse(parentDirName, parentDirUri, DirectoryStatus.Online);
                await this.AddToProductionTable(resObj, productionReq);

                try
                {
                    await this.CreateDirectoryAndSetAccessControl(fileShareDirectory, dir.Type, parentDirName, prodStore);
                    await this.CreateSubDirectoryTree(dir, parentDirectory);
                }
                catch (Exception)
                {
                    await this.productionRepository.DeleteProduction(resObj.Id);
                    throw;
                }
            }

            return resObj;
        }

        /// <inheritdoc/>
        public async Task<ProductionResponse> DeleteProduction(string productionStoreId, string productionId)
        {
            ProductionResponse resObj = new ProductionResponse();

            var production = await this.productionRepository.GetProductionById(productionId);
            if (production == null || production.ProductionStoreId != productionStoreId)
            {
                throw new DirectoryNotFoundException($"Production store : {productionStoreId} Production: {productionId} entry was not found in production table.");
            }

            if (production.Status == "Online")
            {
                throw new InvalidOperationException($"{production.Name} cannot be deleted since the status is online.");
            }

            if (production.DeletedFlag == true)
            {
                throw new InvalidOperationException($"{production.Name} is already deleted.");
            }

            production.DeletedFlag = true;
            production.DeletedDateTime = DateTime.UtcNow;

            await this.productionRepository.UpdateProduction(production);

            resObj.Id = production.Id;
            resObj.Name = production.Name;
            resObj.Status = production.Status;
            resObj.DeletedFlag = production.DeletedFlag;
            resObj.CreatedDateTime = production.CreatedDateTime;
            resObj.LastSyncDateTime = production.LastSyncDateTime;
            resObj.Uri = production.ArchiveUrl;
            resObj.DeletedDateTime = production.DeletedDateTime;
            return resObj;
        }

        private static bool IsStoreManager(List<string> userGroups, string storeManagerGroups)
        {
            var managerGroupArray = storeManagerGroups.Split(",");
            foreach (string group in managerGroupArray)
            {
                if (userGroups.Any(x => x.Equals(group.Trim())))
                {
                    return true;
                }
            }

            return false;
        }

        private ProductionResponse CreateProductionResponse(string dirName, string uri, DirectoryStatus status)
        {
            ProductionResponse res = new ProductionResponse();

            res.Id = Guid.NewGuid().ToString();
            res.Name = dirName;
            res.Uri = uri;
            res.Status = status.ToString();
            res.CreatedDateTime = DateTime.UtcNow;
            res.DeletedFlag = false;
            return res;
        }

        private void ThrowDuplicateException(List<string> duplicateDirectories)
        {
            string duplicateStr = string.Join(", ", duplicateDirectories).RemoveSlash();
            throw new DuplicateNameException($"Duplicate name not allowed: {duplicateStr}");
        }

        private async Task CreateSubDirectoryTree(DirectoryTree dir, ShareDirectoryClient parentDirectory)
        {
            this.log.LogInformation($"Create sub directories started.");
            if (dir.SubItems.Any())
            {
                foreach (var subItem in dir.SubItems)
                {
                    string subDirName = subItem.Path.Trim().RemoveSpecialChars();
                    await this.CreateDirectoryFromParent(parentDirectory, subItem.Type, subDirName);
                    ShareDirectoryClient subDirectory = parentDirectory.GetSubdirectoryClient(subDirName);
                    await this.TraverseDirectoryTree(subDirectory, subItem);
                }
            }

            this.log.LogInformation($"Create sub directories completed.");
        }

        private async Task CreateDirectoryAndSetAccessControl(ShareClient fileShareDirectory, ItemType type, string dirName, ProductionStore productionStore)
        {
            if (type.ToString().ToLower().Equals(ItemType.Folder.ToString().ToLower()))
            {
                this.log.LogInformation($"Build custom SDDL started.");
                string sddl = this.sddlBuilderUtility.BuildSDDL(productionStore, ActivityType.Create);
                string filePermission = sddl;
                ShareDirectoryClient customDirectory = fileShareDirectory.GetDirectoryClient(dirName);
                this.log.LogInformation($"Build custom SDDL completed.");

                this.log.LogInformation($"Create Production directory started.");
                await customDirectory.CreateIfNotExistsAsync(filePermission: filePermission);
                this.log.LogInformation($"Create Production directory completed.");
            }
        }

        private async Task CreateDirectoryFromParent(ShareDirectoryClient fileDirectory, ItemType type, string dirName)
        {
            if (type.ToString().ToLower().Equals(ItemType.Folder.ToString().ToLower()))
            {
                ShareDirectoryClient customDirectory = fileDirectory.GetSubdirectoryClient(dirName);
                await customDirectory.CreateIfNotExistsAsync();
            }
        }

        private void CheckDuplicateDirectoryName(ProductionRequest prouctionReq)
        {
            foreach (var dir in prouctionReq.DirectoryTree)
            {
                var drResult = dir.SubItems.GroupBy(x => x.Path.Trim().ToLower()).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (!drResult.Any())
                {
                    foreach (var subItem in dir.SubItems)
                    {
                        drResult = subItem.SubItems.GroupBy(x => x.Path.Trim().ToLower()).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                        if (!drResult.Any())
                        {
                            this.CheckDuplicateSubDirectory(subItem);
                        }
                        else
                        {
                            this.ThrowDuplicateException(drResult.ToList());
                        }
                    }
                }
                else
                {
                    this.ThrowDuplicateException(drResult.ToList());
                }
            }
        }

        private async Task TraverseDirectoryTree(ShareDirectoryClient fileDirectory, SubItem subItem)
        {
            foreach (var item in subItem.SubItems)
            {
                string subDirName = item.Path.Trim().RemoveSpecialChars();
                await this.CreateDirectoryFromParent(fileDirectory, item.Type, item.Path.Trim().RemoveSpecialChars());
                ShareDirectoryClient subDirectory = fileDirectory.GetSubdirectoryClient(subDirName);
                await this.TraverseDirectoryTree(subDirectory, item); // Recurse here
            }
        }

        private void CheckDuplicateSubDirectory(SubItem subItem)
        {
            foreach (var item in subItem.SubItems)
            {
                var subDir = item.SubItems.GroupBy(x => x.Path.Trim().ToLower()).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                if (!subDir.Any())
                {
                    this.CheckDuplicateSubDirectory(item); // Recurse here
                }
                else
                {
                    this.ThrowDuplicateException(subDir.ToList());
                }
            }
        }

        private async Task AddToProductionTable(ProductionResponse response, ProductionRequest productionReq)
        {
            string prodNameUri = response.Uri.Split("/")[2];
            var production = new Wpp.StorageAutomation.Entities.Models.Production
            {
                Id = response.Id,
                Name = response.Name,
                ProductionStoreId = productionReq.ProductionStoreId,
                Wipurl = response.Uri,
                Status = response.Status,
                ArchiveUrl = response.Uri.Replace(prodNameUri, response.Id),
                CreatedDateTime = response.CreatedDateTime,
                ModifiedDateTime = response.CreatedDateTime,
                DeletedFlag = false
            };

            await this.productionRepository.AddProduction(production);
        }
    }
}
