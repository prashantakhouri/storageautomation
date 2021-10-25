// <copyright file="IDataMovement.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Wpp.StorageAutomation.DataMovement.Models;

namespace Wpp.StorageAutomation.DataMovement.Contracts
{
    /// <summary>
    ///   The data movement.
    /// </summary>
    public interface IDataMovement
    {
        /// <summary>
        /// Archives all production stores.
        /// </summary>
        /// <returns>A task.</returns>
        Task<ArchiveAllResponse> ArchiveAsync();

        /// <summary>
        /// Archives a production store.
        /// </summary>
        /// <param name="productionStoreId">The production store Id.</param>
        /// <returns>
        /// A task.
        /// </returns>
        Task<ArchiveProductionStoreResponse> ArchiveProductionStoreAsync(string productionStoreId);

        /// <summary>
        /// Archives a production.
        /// </summary>
        /// <param name="archiveProductionRequest">The archive production request.</param>
        /// <returns>
        /// A task.
        /// </returns>
        Task<ArchiveProductionResponse> ArchiveProductionAsync(ProductionRequest archiveProductionRequest);

        /// <summary>Restores Production selected in a Productionstore.</summary>
        /// <param name="request">The request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<ProductionResponse> RestoreProductionAsync(ProductionRequest request);

        /// <summary>
        /// Makes the production offline asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A response.</returns>
        Task<ProductionResponse> MakeProductionOfflineAsync(ProductionRequest request);
    }
}
