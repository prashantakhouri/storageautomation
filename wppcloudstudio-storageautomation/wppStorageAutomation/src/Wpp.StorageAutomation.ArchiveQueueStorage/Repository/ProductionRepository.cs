// <copyright file="ProductionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Entities.Models;
using ProductionEntity = Wpp.StorageAutomation.Entities.Models.Production;

namespace Wpp.StorageAutomation.ArchiveQueueStorage
{
    /// <summary>
    /// Production repository.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.Entities.Contract.IProductionRepository" />
    public class ProductionRepository : IProductionRepository
    {
        private readonly DbContextOptionsBuilder<WppsqldbContext> optionsBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionRepository" /> class.
        /// </summary>
        public ProductionRepository()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<WppsqldbContext>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Production>> GetOnlineProductionsByProdStoreId(string productionStoreId)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                return await db.Production.Where(x => x.ProductionStoreId == productionStoreId && (x.Status == DirectoryStatus.Online.ToString() || x.Status == DirectoryStatus.Archiving.ToString())).ToListAsync() ?? null;
            }
        }

        /// <inheritdoc/>
        public async Task UpdateProduction(Production production)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                db.Production.Update(production);
                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task UpdateLastSyncDate(string productionId, DateTime lastSyncDate)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                db.Production
                .Where(x => x.Id == productionId)
                .ToList()
                .ForEach(x => x.LastSyncDateTime = lastSyncDate);

                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task UpdateArchiveGetStatusQueryUri(string getStatusQueryUri)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                db.Production
                .Where(x => x.Status == DirectoryStatus.Online.ToString() || x.Status == DirectoryStatus.Archiving.ToString())
                .ToList()
                .ForEach(x => x.GetStatusQueryUri = getStatusQueryUri);

                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task UpdateArchiveGetStatusQueryUri(string productionStoreId, string getStatusQueryUri)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                db.Production
                .Where(x => x.ProductionStoreId == productionStoreId)
                .Where(x => x.Status == DirectoryStatus.Online.ToString() || x.Status == DirectoryStatus.Archiving.ToString())
                .ToList()
                .ForEach(x => x.GetStatusQueryUri = getStatusQueryUri);

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
        public async Task<ProductionEntity> GetProductionById(string productionId)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                var production = await db.Production.Where(x => x.Id == productionId).FirstOrDefaultAsync();
                return production;
            }
        }

        /// <inheritdoc/>
        public async Task UpdateStatus(string fromStatus, string toStatus)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                db.Production
                    .Where(x => x.Status == fromStatus)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.Status = toStatus;
                        x.StateChangeDateTime = DateTime.UtcNow;
                    });

                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task UpdateStatusOfProductionStore(string productionStoreId, string fromStatus, string toStatus)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                db.Production
                    .Where(x => x.ProductionStoreId == productionStoreId && x.Status == fromStatus)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.Status = toStatus;
                        x.StateChangeDateTime = DateTime.UtcNow;
                    });

                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task UpdateStatusOfProduction(string productionId, string fromStatus, string toStatus)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                db.Production
                    .Where(x => x.Id == productionId && x.Status == fromStatus)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.Status = toStatus;
                        x.StateChangeDateTime = DateTime.UtcNow;
                    });

                await db.SaveChangesAsync();
            }
        }
    }
}
