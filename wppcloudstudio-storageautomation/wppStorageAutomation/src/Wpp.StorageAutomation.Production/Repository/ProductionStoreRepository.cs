// <copyright file="ProductionStoreRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wpp.StorageAutomation.Entities.Models;
using ProductionStoreEntity = Wpp.StorageAutomation.Entities.Models.ProductionStore;

namespace Wpp.StorageAutomation.Production
{
    /// <summary>
    /// Production store detail repository.
    /// </summary>
    /// <seealso cref="IProductionStoreRepository" />
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

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductionStore>> GetAllProductionStores()
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                return await db.ProductionStore.ToListAsync() ?? null;
            }
        }

        /// <inheritdoc/>
        public async Task<ProductionStore> GetProductionStoreByName(string productionStoreName)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                return await db.ProductionStore.FirstOrDefaultAsync(x => x.Name == productionStoreName);
            }
        }

        /// <inheritdoc/>
        public async Task<ProductionStore> GetProductionStoreById(string productionStoreId)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                return await db.ProductionStore.FirstOrDefaultAsync(x => x.Id == productionStoreId);
            }
        }

        /// <inheritdoc/>
        public async Task AddProductionStore(ProductionStore productionStoreDetail)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                await db.ProductionStore.AddAsync(productionStoreDetail);
                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task DeleteProductionStore(string id)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                var productionStoreDetail = await db.ProductionStore.FirstOrDefaultAsync(x => x.Id == id);

                if (productionStoreDetail != null)
                {
                    db.ProductionStore.Remove(productionStoreDetail);
                    await db.SaveChangesAsync();
                }
            }
        }

        /// <inheritdoc/>
        public async Task UpdateProductionStore(ProductionStore productionStore)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                db.ProductionStore.Update(productionStore);
                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ProductionStoreExists(string productionStoreId)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                return await db.ProductionStore.AnyAsync(x => x.Id == productionStoreId);
            }
        }

        /// <inheritdoc/>
        public async Task<string> ReadStorageKey(string productionStoreId)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                return await db.ProductionStore.Where(x => x.Id == productionStoreId).Select(x => x.WipkeyName).FirstOrDefaultAsync();
            }
        }
    }
}
