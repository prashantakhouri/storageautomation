// <copyright file="IProductionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionEntity = Wpp.StorageAutomation.Entities.Models.Production;

namespace Wpp.StorageAutomation.Production
{
    /// <summary>
    /// Production repository.
    /// </summary>
    public interface IProductionRepository
    {
        /// <summary>
        /// Gets all productions.
        /// </summary>
        /// <returns>A list of all productions.</returns>
        Task<IEnumerable<ProductionEntity>> GetAllProductions();

        /// <summary>
        /// Adds the production.
        /// </summary>
        /// <param name="production">The production.</param>
        /// <returns>The task.</returns>
        Task AddProduction(ProductionEntity production);

        /// <summary>
        /// Deletes the production.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task.</returns>
        Task DeleteProduction(string id);

        /// <summary>
        /// Updates the production.
        /// </summary>
        /// <param name="production">The production.</param>
        /// <returns>The task.</returns>
        Task UpdateProduction(ProductionEntity production);

        /// <summary>
        /// Productions the exists.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="productionName">Name of the production.</param>
        /// <returns>A Production with specified Production Store Id and name.</returns>
        Task<ProductionEntity> GetProductionByName(string productionStoreId, string productionName);

        /// <summary>
        /// Gets the production by production store identifier.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>List of Productions belonging to a Production Store Id.</returns>
        Task<IEnumerable<ProductionEntity>> GetProductionByProductionStoreId(string productionStoreId);

        /// <summary>
        /// Checks if a production name exists.
        /// </summary>
        /// <param name="productionName">The production name.</param>
        /// <returns> A boolean value.</returns>
        Task<bool> ProductionExists(string productionName);

        /// <summary>
        /// Gets the production by identifier.
        /// </summary>
        /// <param name="productionId">The production identifier.</param>
        /// <returns>The production.</returns>
        Task<ProductionEntity> GetProductionById(string productionId);
    }
}