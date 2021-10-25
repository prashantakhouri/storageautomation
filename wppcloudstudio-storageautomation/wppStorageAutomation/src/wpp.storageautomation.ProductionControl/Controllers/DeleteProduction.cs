// <copyright file="DeleteProduction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Wpp.StorageAutomation
{
    /// <summary>
    ///   The delete production controller.
    /// </summary>
    public static class DeleteProduction
    {
        /// <summary>Deletes the production starter.</summary>
        /// <param name="req">The req.</param>
        /// <param name="starter">The starter.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [FunctionName(nameof(DeleteProductionStarter))]
        public static async Task<HttpResponseMessage> DeleteProductionStarter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DeleteProductionOrchestrator", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}