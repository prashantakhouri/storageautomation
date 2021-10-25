// <copyright file="IProductionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wpp.StorageAutomation.Entities.Models;

namespace Wpp.StorageAutomation.ArchiveScheduler
{
    /// <summary>
    /// Production repository.
    /// </summary>
    public interface IProductionRepository
    {
        /// <summary>
        /// Gets all online productions.
        /// </summary>
        /// <param name="durationTime"> The durationTime.</param>
        /// <returns>The production list.</returns>
        Task<IEnumerable<Production>> GetAllValidProductions(double durationTime);
    }
}
