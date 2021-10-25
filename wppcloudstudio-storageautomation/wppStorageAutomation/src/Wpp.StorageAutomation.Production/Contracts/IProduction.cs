// <copyright file="IProduction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wpp.StorageAutomation.Production
{
    /// <summary>
    /// Production.
    /// </summary>
    public interface IProduction
    {
        /// <summary>
        /// Gets the productions by production store asynchronous.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="userGroups">The groups.</param>
        /// <returns>Production list.</returns>
        Task<ProductionListResponse> GetProductionsByProductionStoreAsync(string productionStoreId, List<string> userGroups);

        /// <summary>
        /// Creates the production.
        /// </summary>
        /// <param name="productionReq">The production req.</param>
        /// <returns>Response after creating a production.</returns>
        Task<ProductionResponse> CreateProduction(ProductionRequest productionReq);

        /// <summary>
        /// Deletes a production.
        /// </summary>
        /// <param name="productionStoreId">The production store id.</param>
        /// <param name="productionId">The production id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<ProductionResponse> DeleteProduction(string productionStoreId, string productionId);
    }
}
