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

namespace Wpp.StorageAutomation.ArchiveScheduler
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
        public async Task<IEnumerable<Production>> GetAllValidProductions(double durationTime)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                DateTime currentTime = DateTime.UtcNow.AddMinutes(-durationTime);
                return await db.Production.Where(x => (x.Status == DirectoryStatus.Archiving.ToString() && x.StateChangeDateTime < currentTime) || x.Status == DirectoryStatus.Online.ToString()).ToListAsync() ?? null;
            }
        }
    }
}