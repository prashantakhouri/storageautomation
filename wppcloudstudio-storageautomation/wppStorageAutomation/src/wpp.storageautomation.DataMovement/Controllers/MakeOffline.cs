// <copyright file="MakeOffline.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Wpp.StorageAutomation.DataMovement.Models;
using Wpp.StorageAutomation.DataMovement.Repository;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation.DataMovement
{
    /// <summary>
    ///   The make offline production controller.
    /// </summary>
    public class MakeOffline
    {
        private readonly IBaseSecurity baseSecurity;
        private readonly IProductionStoreRepository productionStoreRepository;
        private readonly IProductionRepository productionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MakeOffline" /> class.
        /// </summary>
        /// <param name="baseSecurity">The base security.</param>
        /// <param name="productionStoreRepository">The production store repository.</param>
        /// <param name="productionRepository">The production repository.</param>
        public MakeOffline(IBaseSecurity baseSecurity, IProductionStoreRepository productionStoreRepository, IProductionRepository productionRepository)
        {
            this.baseSecurity = baseSecurity;
            this.productionStoreRepository = productionStoreRepository;
            this.productionRepository = productionRepository;
        }

        /// <summary>
        /// Make production offline.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="productionId">The production identifier.</param>
        /// <param name="starter">The starter.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        /// Http response message.
        /// </returns>
        [FunctionName(nameof(MakeProductionOffline))]
        public async Task<IActionResult> MakeProductionOffline(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "production-stores/{productionStoreId}/productions/{productionId}/make-offline")] HttpRequest request,
            string productionStoreId,
            string productionId,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("MakeProductionOffline API triggered.");

            var prodStore = await this.productionStoreRepository.GetProductionStoreById(productionStoreId);
            if (prodStore == null)
            {
                return new ObjectResult(new { id = string.Empty, name = string.Empty, error = $"No production store exists with: {productionStoreId} id." }) { StatusCode = (int)HttpStatusCode.NotFound };
            }

            var userAccess = this.baseSecurity.ValidateUserGroupsAccess(request, log, productionStoreId);
            if (userAccess.HasAccess)
            {
                var productionInfo = new ProductionRequest()
                {
                    ProductionStoreId = productionStoreId,
                    ProductionId = productionId
                };

                string instanceId = await starter.StartNewAsync("MakeProductionOfflineOrchestrator", productionInfo);
                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

                var response = starter.CreateCheckStatusResponse(request, instanceId);
                var okObjectResult = response as ObjectResult;
                var value = okObjectResult.Value as HttpResponseMessage;
                var content = await value.Content.ReadAsStringAsync();
                var body = JsonSerializer.Deserialize<DurableResponseBody>(content);

                var funcStatusUri = new Uri(body.statusQueryGetUri);
                var appendPath = funcStatusUri.ToString().Split("instances/", StringSplitOptions.RemoveEmptyEntries)[1];
                string apimBaseUri = Environment.GetEnvironmentVariable("APIM_BASE_URL");
                var apimGetStatusQuery = apimBaseUri + appendPath;

                var production = await this.productionRepository.GetProductionById(productionId);
                if (production != null)
                {
                    production.GetStatusQueryUri = apimGetStatusQuery;
                    await this.productionRepository.UpdateProduction(production);
                }

                return starter.CreateCheckStatusResponse(request, instanceId);
            }
            else
            {
                log.LogError("Access Forbidden to MakeProductionOffline API.");
                return new ObjectResult(new { id = prodStore.Id, name = prodStore.Name }) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
        }
    }
}