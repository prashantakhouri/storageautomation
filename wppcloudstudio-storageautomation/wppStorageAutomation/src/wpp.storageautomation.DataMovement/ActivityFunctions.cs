// <copyright file="ActivityFunctions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Wpp.StorageAutomation.DataMovement.Contracts;
using Wpp.StorageAutomation.DataMovement.Models;

namespace Wpp.StorageAutomation.DataMovement
{
    /// <summary>
    ///   The activity functions class.
    /// </summary>
    public class ActivityFunctions
    {
        private readonly IDataMovement dataMovement;

        /// <summary>Initializes a new instance of the <see cref="ActivityFunctions" /> class.</summary>
        /// <param name="dataMovement">The data movement.</param>
        public ActivityFunctions(IDataMovement dataMovement)
        {
           this.dataMovement = dataMovement;
        }

        /// <summary>
        /// Archives the asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="log">The log.</param>
        /// <returns>The response.</returns>
        [FunctionName(nameof(ArchiveAsync))]
        public async Task<ArchiveAllResponse> ArchiveAsync([ActivityTrigger] string name, ILogger log)
        {
            return await this.dataMovement.ArchiveAsync();
        }

        /// <summary>
        /// Archives the production store asynchronous.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="log">The log.</param>
        /// <returns>The response.</returns>
        [FunctionName(nameof(ArchiveProductionStoreAsync))]
        public async Task<ArchiveProductionStoreResponse> ArchiveProductionStoreAsync([ActivityTrigger] string productionStoreId, ILogger log)
        {
            return await this.dataMovement.ArchiveProductionStoreAsync(productionStoreId);
        }

        /// <summary>
        /// Archives the production asynchronous.
        /// </summary>
        /// <param name="archiveProductionRequest">The archive production request.</param>
        /// <param name="log">The log.</param>
        /// <returns>The response.</returns>
        [FunctionName(nameof(ArchiveProductionAsync))]
        public async Task<ArchiveProductionResponse> ArchiveProductionAsync([ActivityTrigger] ProductionRequest archiveProductionRequest, ILogger log)
        {
            return await this.dataMovement.ArchiveProductionAsync(archiveProductionRequest);
        }

        /// <summary>Restores the production.</summary>
        /// <param name="request">The request.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        ///   The response.
        /// </returns>
        [FunctionName(nameof(RestoreProductionAsync))]
        public async Task<ProductionResponse> RestoreProductionAsync([ActivityTrigger] ProductionRequest request, ILogger log)
        {
            return await this.dataMovement.RestoreProductionAsync(request);
        }

        /// <summary>Makes the production offline.</summary>
        /// <param name="request">The request.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        ///   The response.
        /// </returns>
        [FunctionName(nameof(MakeProductionOfflineAsync))]
        public async Task<ProductionResponse> MakeProductionOfflineAsync([ActivityTrigger] ProductionRequest request, ILogger log)
        {
            return await this.dataMovement.MakeProductionOfflineAsync(request);
        }
    }
}