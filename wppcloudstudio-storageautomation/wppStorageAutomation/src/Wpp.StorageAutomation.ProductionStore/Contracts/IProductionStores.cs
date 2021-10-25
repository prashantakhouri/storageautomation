// <copyright file="IProductionStores.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    /// Production stores.
    /// </summary>
    public interface IProductionStores
    {
        /// <summary>
        /// Saves the production store.
        /// </summary>
        /// <param name="productionStoreRequest">The production store request.</param>
        /// <returns>Production store response.</returns>
        Task<ProductionStoreResponse> SaveProductionStore(ProductionStoreRequest productionStoreRequest);

        /// <summary>
        /// Lists the production stores asynchronous.
        /// </summary>
        /// <param name="userGroups">The user groups.</param>
        /// <returns>A list of production stores.</returns>
        Task<ProductionStoreListResponse> ListProductionStoresAsync(List<string> userGroups);
    }
}
