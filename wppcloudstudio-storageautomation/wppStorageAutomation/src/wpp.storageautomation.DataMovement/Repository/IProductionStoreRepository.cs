// <copyright file="IProductionStoreRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Wpp.StorageAutomation.Entities.Models;

namespace Wpp.StorageAutomation.DataMovement.Repository
{
    /// <summary>
    /// Production store detail repository.
    /// </summary>
    public interface IProductionStoreRepository
    {
        /// <summary>
        /// Gets all production store details.
        /// </summary>
        /// <returns>A list of all production store details.</returns>
        Task<IEnumerable<ProductionStore>> GetAllProductionStores();

        /// <summary>
        /// Gets the production store.
        /// </summary>
        /// <param name="productionStoreName">Name of the production store.</param>
        /// <returns>The production store.</returns>
        Task<ProductionStore> GetProductionStoreByName(string productionStoreName);

        /// <summary>
        /// Gets the production store by identifier.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>The production store.</returns>
        Task<ProductionStore> GetProductionStoreById(string productionStoreId);

        /// <summary>
        /// Adds the production store details.
        /// </summary>
        /// <param name="productionStoreDetail">The production store detail.</param>
        /// <returns>The task.</returns>
        Task AddProductionStore(ProductionStore productionStoreDetail);

        /// <summary>
        /// Deletes the production store details.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The task.</returns>
        Task DeleteProductionStore(string id);

        /// <summary>
        /// Updates the production.
        /// </summary>
        /// <param name="productionStoreDetail">The production store detail.</param>
        /// <returns>The task.</returns>
        Task UpdateProductionStore(ProductionStore productionStoreDetail);
    }
}