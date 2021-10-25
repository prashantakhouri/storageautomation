// <copyright file="IProduction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace Wpp.StorageAutomation
{
    /// <summary>
    ///   <br />
    /// </summary>
    public interface IProduction
    {
        /// <summary>Creates the production.</summary>
        /// <param name="productionReq">The production req.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        Task<ProductionResponse> CreateProduction(ProductionRequest productionReq);

        /// <summary>
        /// Gets the productions by production store asynchronous.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>A list of productions belonging to a production store.</returns>
        Task<ProductionListResponse> GetProductionsByProductionStoreAsync(string productionStoreId);
    }
}
