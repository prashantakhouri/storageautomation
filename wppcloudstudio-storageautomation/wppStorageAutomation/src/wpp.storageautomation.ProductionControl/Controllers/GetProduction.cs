// <copyright file="GetProduction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation
{
    /// <summary>
    ///   The get production controller.
    /// </summary>
    public static class GetProduction
    {
        /// <summary>Gets the GetProductionStarter starter.</summary>
        /// <param name="req">The req.</param>
        /// <param name="id">The log.</param>
        /// <param name="starter">The starter.</param>
        /// <param name="log">The logger.</param>
        /// <returns>
        ///   A list of production information.
        /// </returns>
        [FunctionName(nameof(GetProductionStarter))]
        public static async Task<IActionResult> GetProductionStarter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Productions/{id}")] HttpRequest req,
            string id,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
                    string instanceId = await starter.StartNewAsync<string>("GetProductionOrchestrator", id);
                    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
                    return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}