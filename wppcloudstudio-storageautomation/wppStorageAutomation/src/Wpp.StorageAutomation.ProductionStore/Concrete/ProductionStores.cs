// <copyright file="ProductionStores.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Logging;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Entities.Models;
using ProductionStoreEntity = Wpp.StorageAutomation.Entities.Models.ProductionStore;

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    /// ProductionStores.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.ProductionStore.IProductionStores" />
    public class ProductionStores : IProductionStores
    {
        private readonly ICloudStorageUtility cloudStorageUtility;
        private readonly IStorageAccountConfig storageAccountConfig;
        private readonly IProductionStoreRepository productionStoreRepository;
        private readonly IGroupsRepository groupsRepository;
        private readonly ILogger<ProductionStores> log;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionStores"/> class.
        /// </summary>
        /// <param name="storageAccountConfig">The storage account configuration.</param>
        /// <param name="cloudStorageUtility">The cloud storage utility.</param>
        /// <param name="productionRepository">The production repository.</param>
        /// <param name="productionStoreRepository">The production store repository.</param>
        /// <param name="groupsRepository">The groups repository.</param>
        /// <param name="log">The log.</param>
        public ProductionStores(
            IStorageAccountConfig storageAccountConfig,
            ICloudStorageUtility cloudStorageUtility,
            IProductionStoreRepository productionStoreRepository,
            IGroupsRepository groupsRepository,
            ILogger<ProductionStores> log)
        {
            this.storageAccountConfig = storageAccountConfig;
            this.cloudStorageUtility = cloudStorageUtility;
            this.productionStoreRepository = productionStoreRepository;
            this.groupsRepository = groupsRepository;
            this.log = log;
        }

        /// <summary>
        /// Lists the production stores asynchronous.
        /// </summary>
        /// <param name="userGroups">The user groups.</param>
        /// <returns>Returns list of production stores.</returns>
        public async Task<ProductionStoreListResponse> ListProductionStoresAsync(List<string> userGroups)
        {
            var prodStores = await this.productionStoreRepository.GetAllProductionStores(userGroups);
            var prodStoreListResponse = new ProductionStoreListResponse()
            {
                ProductionStoreList = prodStores.Select(x => new ProductionStoreRow()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Region = x.Region,
                    Wipurl = x.Wipurl,
                    WipallocatedSize = x.WipallocatedSize,
                    ArchiveUrl = x.ArchiveUrl,
                    ArchiveAllocatedSize = x.ArchiveAllocatedSize,
                    ScaleDownTime = x.ScaleDownTime,
                    ScaleUpTimeInterval = x.ScaleUpTimeInterval,
                    MinimumFreeSize = x.MinimumFreeSize,
                    MinimumFreeSpace = x.MinimumFreeSpace,
                    OfflineTime = x.OfflineTime,
                    OnlineTime = x.OnlineTime,
                    ProductionOfflineTimeInterval = x.ProductionOfflineTimeInterval,
                    ManagerRoleGroupNames = x.ManagerRoleGroupNames,
                    UserRoleGroupNames = x.UserRoleGroupNames,
                    WipkeyName = x.WipkeyName,
                    ArchiveKeyName = x.ArchiveKeyName
                })
            };

            return prodStoreListResponse;
        }

        /// <inheritdoc/>
        public async Task<ProductionStoreResponse> SaveProductionStore(ProductionStoreRequest productionStoreRequest)
        {
            bool storeRecordExists = await this.productionStoreRepository.ProductionStoreExists(productionStoreRequest.Name);

            if (storeRecordExists)
            {
                throw new DuplicateNameException($"Production store record already exists. Production Store Name : {productionStoreRequest.Name}");
            }

            var wipConnectionString = this.storageAccountConfig.GetStorageConnectionString(productionStoreRequest.WipkeyName);
            this.storageAccountConfig.GetStorageConnectionString(productionStoreRequest.ArchiveKeyName);
            var storeExists = await this.FileShareExists(wipConnectionString, productionStoreRequest.Name);

            if (!storeExists)
            {
                throw new KeyNotFoundException($"Production store: {productionStoreRequest.Name} does not exist in the WIP storage account.");
            }

            var productionStoreEntity = new ProductionStoreEntity()
            {
                Id = Guid.NewGuid().ToString(),
                Name = productionStoreRequest.Name,
                Region = productionStoreRequest.Region,
                Wipurl = productionStoreRequest.WIPURL,
                WipallocatedSize = productionStoreRequest.WIPAllocatedSize,
                ArchiveUrl = productionStoreRequest.ArchiveURL,
                ArchiveAllocatedSize = productionStoreRequest.ArchiveAllocatedSize,
                ManagerRoleGroupNames = productionStoreRequest.ManagerRoleGroupNames,
                UserRoleGroupNames = productionStoreRequest.UserRoleGroupNames,
                WipkeyName = productionStoreRequest.WipkeyName,
                ArchiveKeyName = productionStoreRequest.ArchiveKeyName
            };

            if (!string.IsNullOrEmpty(productionStoreRequest.ManagerRoleGroupSIDs))
            {
                List<Groups> groupList = new List<Groups>();
                var managerGroupSIDs = productionStoreRequest.ManagerRoleGroupSIDs.Split(',');
                var managerGroupNames = productionStoreRequest.ManagerRoleGroupNames.Split(',');
                if (managerGroupSIDs.Count() != managerGroupNames.Count())
                {
                    throw new ArgumentException($"The 'ManagerRoleGroupSIDs' count does not match 'ManagerRoleGroupNames' count.");
                }

                foreach (var (groupSid, groupName) in managerGroupSIDs.Zip(managerGroupNames))
                {
                    Groups group = new Groups
                    {
                        GroupSid = groupSid,
                        GroupName = groupName
                    };
                    groupList.Add(group);
                }

                await this.groupsRepository.AddGroupRange(groupList);
            }

            if (!string.IsNullOrEmpty(productionStoreRequest.UserRoleGroupSIDs))
            {
                List<Groups> groupList = new List<Groups>();
                var userGroupSIDs = productionStoreRequest.UserRoleGroupSIDs.Split(',');
                var userGroupNames = productionStoreRequest.UserRoleGroupNames.Split(',');
                if (userGroupSIDs.Count() != userGroupNames.Count())
                {
                    throw new ArgumentException($"The 'UserRoleGroupSIDs' count does not match 'UserRoleGroupNames' count.");
                }

                foreach (var (groupSid, groupName) in userGroupSIDs.Zip(userGroupNames))
                {
                    Groups group = new Groups
                    {
                        GroupSid = groupSid,
                        GroupName = groupName
                    };
                    groupList.Add(group);
                }

                await this.groupsRepository.AddGroupRange(groupList);
            }

            await this.productionStoreRepository.AddProductionStore(productionStoreEntity);
            ProductionStoreResponse response = new ProductionStoreResponse
            {
                Id = productionStoreEntity.Id,
                Name = productionStoreEntity.Name,
                CreatedDate = DateTime.UtcNow
            };
            return response;
        }

        private async Task<bool> FileShareExists(string wipConnectionString, string productionStoreName)
        {
            var fileshareClient = new ShareServiceClient(wipConnectionString);
            bool fileShareExists = await fileshareClient.GetShareClient(productionStoreName).ExistsAsync();
            return fileShareExists;
        }
    }
}
