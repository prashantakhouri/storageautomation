// <copyright file="IProductionStoreRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionStoreEntity = Wpp.StorageAutomation.Entities.Models.ProductionStore;

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    /// Production store repository.
    /// </summary>
    public interface IProductionStoreRepository
    {
        /// <summary>
        /// Adds all production store details.
        /// </summary>
        /// <param name="productionStore">The production store.</param>
        /// <returns>
        /// The task.
        /// </returns>
        Task AddProductionStore(ProductionStoreEntity productionStore);

        /// <summary>
        /// Gets the name of the production store by.
        /// </summary>
        /// <param name="productionStoreName">Name of the production store.</param>
        /// <returns>A productions store.</returns>
        Task<bool> ProductionStoreExists(string productionStoreName);

        /// <summary>
        /// Gets all production stores.
        /// </summary>
        /// <param name="userGroups">The user groups.</param>
        /// <returns>Returns list of production stores.</returns>
        Task<IEnumerable<ProductionStoreEntity>> GetAllProductionStores(List<string> userGroups);
    }
}
