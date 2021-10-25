// <copyright file="IProductionStoreRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Wpp.StorageAutomation.Entities.Models;
using ProductionStoreEntity = Wpp.StorageAutomation.Entities.Models.ProductionStore;

namespace Wpp.StorageAutomation.Production
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
        /// Gets the name of the production store by.
        /// </summary>
        /// <param name="productionStoreName">Name of the production store.</param>
        /// <returns>A productions store.</returns>
        Task<ProductionStore> GetProductionStoreByName(string productionStoreName);

        /// <summary>
        /// Gets the production store by identifier.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>The production store information.</returns>
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
        /// <param name="productionStore">The production store.</param>
        /// <returns>
        /// The task.
        /// </returns>
        Task UpdateProductionStore(ProductionStore productionStore);

        /// <summary>
        /// Checks if production store exists.
        /// </summary>
        /// <param name="productionStoreId">The production store id.</param>
        /// <returns>
        /// The task.
        /// </returns>
        Task<bool> ProductionStoreExists(string productionStoreId);

        /// <summary>
        /// Checks if production store exists.
        /// </summary>
        /// <param name="productionStoreId">The production store id.</param>
        /// <returns>
        /// The storage key.
        /// </returns>
        Task<string> ReadStorageKey(string productionStoreId);
    }
}