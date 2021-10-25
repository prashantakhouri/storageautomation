// <copyright file="IProductionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wpp.StorageAutomation.Entities.Models;

namespace Wpp.StorageAutomation.DataMovement.Repository
{
    /// <summary>
    /// Production repository.
    /// </summary>
    public interface IProductionRepository
    {
        /// <summary>
        /// Gets the online productions by product store identifier.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>A list of Productions.</returns>
        Task<IEnumerable<Production>> GetOnlineProductionsByProdStoreId(string productionStoreId);

        /// <summary>
        /// Updates the production.
        /// </summary>
        /// <param name="production">The production.</param>
        /// <returns>The task.</returns>
        Task UpdateProduction(Production production);

        /// <summary>
        /// Updates the last synchronize date by product store identifier.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="lastSyncDate">The last synchronize date.</param>
        /// <returns>The task.</returns>
        Task UpdateLastSyncDateByProdStoreId(string productionStoreId, DateTime lastSyncDate);

        /// <summary>
        /// Updates the archive get status query URI.
        /// </summary>
        /// <param name="getStatusQueryUri">The get status query URI.</param>
        /// <returns>The task.</returns>
        Task UpdateArchiveGetStatusQueryUri(string getStatusQueryUri);

        /// <summary>
        /// Updates the archive get status query URI.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="getStatusQueryUri">The get status query URI.</param>
        /// <returns>The task.</returns>
        Task UpdateArchiveGetStatusQueryUri(string productionStoreId, string getStatusQueryUri);

        /// <summary>
        /// Gets the name of the production by.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="productionName">Name of the production.</param>
        /// <returns>The production.</returns>
        Task<Wpp.StorageAutomation.Entities.Models.Production> GetProductionByName(string productionStoreId, string productionName);

        /// <summary>
        /// Gets the production by identifier.
        /// </summary>
        /// <param name="productionId">The production identifier.</param>
        /// <returns>The production.</returns>
        Task<Wpp.StorageAutomation.Entities.Models.Production> GetProductionById(string productionId);

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="fromStatus">From status.</param>
        /// <param name="toStatus">To status.</param>
        /// <returns>A task.</returns>
        Task UpdateStatus(string fromStatus, string toStatus);

        /// <summary>
        /// Updates the status of production store.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="fromStatus">From status.</param>
        /// <param name="toStatus">To status.</param>
        /// <returns>A task.</returns>
        Task UpdateStatusOfProductionStore(string productionStoreId, string fromStatus, string toStatus);

        /// <summary>
        /// Updates the status of production.
        /// </summary>
        /// <param name="productionId">The production identifier.</param>
        /// <param name="fromStatus">From status.</param>
        /// <param name="toStatus">To status.</param>
        /// <returns>A task.</returns>
        Task UpdateStatusOfProduction(string productionId, string fromStatus, string toStatus);

        /// <summary>
        /// Gets all online productions.
        /// </summary>
        /// <returns>The production list.</returns>
        Task<IEnumerable<Production>> GetAllOnlineProductions();
    }
}