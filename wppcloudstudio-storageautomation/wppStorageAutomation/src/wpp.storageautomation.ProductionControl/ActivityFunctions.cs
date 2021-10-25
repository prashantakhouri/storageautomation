// <copyright file="ActivityFunctions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Wpp.StorageAutomation
{
    /// <summary>
    ///   The activity functions class.
    /// </summary>
    public class ActivityFunctions
    {
        private readonly IProduction production;

        /// <summary>Initializes a new instance of the <see cref="ActivityFunctions" /> class.</summary>
        /// <param name="production">The production.</param>
        public ActivityFunctions(IProduction production)
        {
            this.production = production;
        }

        /// <summary>
        /// Gets the productions by production store asynchronous.
        /// </summary>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="log">The log.</param>
        /// <returns>List of productions belonging to a production store.</returns>
        [FunctionName(nameof(GetProductionsByProductionStoreAsync))]
        public async Task<ProductionListResponse> GetProductionsByProductionStoreAsync([ActivityTrigger] string productionStoreId, ILogger log)
        {
            return await this.production.GetProductionsByProductionStoreAsync(productionStoreId);
        }

        /// <summary>Creates the production.</summary>
        /// <param name="productionReq">The production req.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [FunctionName(nameof(CreateProduction))]
        public async Task<ProductionResponse> CreateProduction([ActivityTrigger] ProductionRequest productionReq, ILogger log)
        {
            return await this.production.CreateProduction(productionReq);
        }

        /// <summary>Deletes the production.</summary>
        /// <param name="name">The name.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [FunctionName(nameof(DeleteProduction))]
        public string DeleteProduction([ActivityTrigger] string name, ILogger log)
        {
            return $"Hello {name}";
        }
    }
}
