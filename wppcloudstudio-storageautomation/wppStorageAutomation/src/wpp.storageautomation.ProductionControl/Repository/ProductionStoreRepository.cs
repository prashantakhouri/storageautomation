using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wpp.StorageAutomation.Entities.Models;

namespace Wpp.StorageAutomation.ProductionControl.Repository
{
    /// <summary>
    /// Production store detail repository.
    /// </summary>
    /// <seealso cref="IProductionStoreRepository" />
    public class ProductionStoreRepository : IProductionStoreRepository
    {
        private readonly WppsqldbContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionStoreRepository"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        public ProductionStoreRepository(WppsqldbContext db)
        {
            this.db = db;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductionStore>> GetAllProductionStores()
        {
            return await this.db.ProductionStore.ToListAsync() ?? null;
        }

        /// <inheritdoc/>
        public async Task<ProductionStore> GetProductionStoreByName(string productionStoreName)
        {
            return await this.db.ProductionStore.FirstOrDefaultAsync(x => x.Name == productionStoreName);
        }

        /// <inheritdoc/>
        public async Task<ProductionStore> GetProductionStoreById(string productionStoreId)
        {
            return await this.db.ProductionStore.FirstOrDefaultAsync(x => x.Id == productionStoreId);
        }

        /// <inheritdoc/>
        public async Task AddProductionStore(ProductionStore productionStoreDetail)
        {
            await this.db.ProductionStore.AddAsync(productionStoreDetail);
            await this.db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteProductionStore(string id)
        {
            var productionStoreDetail = await this.db.ProductionStore.FirstOrDefaultAsync(x => x.Id == id);

            if (productionStoreDetail != null)
            {
                this.db.ProductionStore.Remove(productionStoreDetail);
                await this.db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task UpdateProductionStore(ProductionStore productionStore)
        {
            this.db.ProductionStore.Update(productionStore);
            await this.db.SaveChangesAsync();
        }
    }
}
