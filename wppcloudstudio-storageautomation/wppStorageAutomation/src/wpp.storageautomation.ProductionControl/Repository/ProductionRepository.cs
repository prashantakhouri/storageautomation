using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wpp.StorageAutomation.Entities.Models;
using ProductionEntity = Wpp.StorageAutomation.Entities.Models.Production;

namespace Wpp.StorageAutomation.ProductionControl.Repository
{
    /// <summary>
    /// Production repository.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.Entities.Contract.IProductionRepository" />
    public class ProductionRepository : IProductionRepository
    {
        private readonly DbContextOptionsBuilder<WppsqldbContext> optionsBuilder;
        private readonly string sqlDbConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionRepository" /> class.
        /// </summary>
        public ProductionRepository()
        {
            this.sqlDbConnectionString = Environment.GetEnvironmentVariable("WPPSQLDBConnection");
            this.optionsBuilder = new DbContextOptionsBuilder<WppsqldbContext>();
            this.optionsBuilder.UseSqlServer(this.sqlDbConnectionString);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductionEntity>> GetAllProductions()
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                return await db.Production.ToListAsync();
            }
        }

        /// <inheritdoc/>
        public async Task AddProduction(ProductionEntity production)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                await db.Production.AddAsync(production);
                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task DeleteProduction(string id)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                var production = await db.Production.FirstOrDefaultAsync(x => x.Id == id);

                if (production != null)
                {
                    db.Production.Remove(production);
                    await db.SaveChangesAsync();
                }
            }
        }

        /// <inheritdoc/>
        public async Task UpdateProduction(ProductionEntity production)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                db.Production.Update(production);
                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<ProductionEntity> GetProductionByName(string productionStoreId, string productionName)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                var production = await db.Production.Where(x => x.ProductionStoreId == productionStoreId && x.Name == productionName).FirstOrDefaultAsync();
                return production;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ProductionEntity>> GetProductionByProductionStoreId(string productionStoreId)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                var production = await db.Production.Where(x => x.ProductionStoreId == productionStoreId).OrderBy(x => x.Name).ToListAsync();
                return production;
            }
        }
    }
}
