// <copyright file="ProductionStoreRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wpp.StorageAutomation.Entities.Models;
using ProductionStoreEntity = Wpp.StorageAutomation.Entities.Models.ProductionStore;

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    /// Production repository.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.ProductionStore.IProductionRepository" />
    public class ProductionStoreRepository : IProductionStoreRepository
    {
        private readonly DbContextOptionsBuilder<WppsqldbContext> optionsBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionStoreRepository"/> class.
        /// </summary>
        public ProductionStoreRepository()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<WppsqldbContext>();
        }

        /// <inheritdoc />
        public async Task AddProductionStore(ProductionStoreEntity productionStore)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                await db.ProductionStore.AddAsync(productionStore);
                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public async Task<bool> ProductionStoreExists(string productionStoreName)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                return await db.ProductionStore.AnyAsync(x => x.Name == productionStoreName);
            }
        }

        /// <summary>
        /// Gets all production stores.
        /// </summary>
        /// <param name="userGroups">The user groups.</param>
        /// <returns>
        /// A list of all production stores.
        /// </returns>
        public Task<IEnumerable<ProductionStoreEntity>> GetAllProductionStores(List<string> userGroups)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                IEnumerable<ProductionStoreEntity> groups = db.ProductionStore.AsEnumerable()
                    .Select(x => new
                    {
                        groupList = x.ManagerRoleGroupNames.Split(",").Concat(x.UserRoleGroupNames.Split(",")).Where(item => userGroups.Contains(item)),
                        x.Id,
                        x.Name,
                        x.Region,
                        x.Wipurl,
                        x.WipallocatedSize,
                        x.ArchiveUrl,
                        x.ArchiveAllocatedSize,
                        x.ScaleDownTime,
                        x.ScaleUpTimeInterval,
                        x.MinimumFreeSize,
                        x.MinimumFreeSpace,
                        x.OfflineTime,
                        x.OnlineTime,
                        x.ProductionOfflineTimeInterval,
                        x.ManagerRoleGroupNames,
                        x.UserRoleGroupNames,
                        x.WipkeyName,
                        x.ArchiveKeyName
                    }).Where(x => x.groupList.Any()).Select(x => new ProductionStoreEntity()
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
                    }).OrderBy(x => x.Region).ThenBy(x => x.Name).ToList();

                return Task.FromResult(groups);
            }
        }
    }
}
