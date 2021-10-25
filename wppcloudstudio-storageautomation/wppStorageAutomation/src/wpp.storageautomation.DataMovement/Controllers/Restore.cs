// <copyright file="Restore.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Linq;
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
using Wpp.StorageAutomation.DataMovement.Models.Validators;
using Wpp.StorageAutomation.DataMovement.Repository;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation.DataMovement
{
    /// <summary>
    ///   The restore production controller.
    /// </summary>
    public class Restore
    {
        private readonly IBaseSecurity baseSecurity;
        private readonly IProductionStoreRepository productionStoreRepository;
        private readonly IProductionRepository productionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Restore" /> class.
        /// </summary>
        /// <param name="baseSecurity">The base security.</param>
        /// <param name="productionStoreRepository">The production store repository.</param>
        /// <param name="productionRepository">The production repository.</param>
        public Restore(IBaseSecurity baseSecurity, IProductionStoreRepository productionStoreRepository, IProductionRepository productionRepository)
        {
            this.baseSecurity = baseSecurity;
            this.productionStoreRepository = productionStoreRepository;
            this.productionRepository = productionRepository;
        }

        /// <summary>
        /// Restore production.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="productionId">The production identifier.</param>
        /// <param name="starter">The starter.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        /// Http response message.
        /// </returns>
        [FunctionName(nameof(RestoreProduction))]
        public async Task<IActionResult> RestoreProduction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "production-stores/{productionStoreId}/productions/{productionId}/restore")] HttpRequest request,
            string productionStoreId,
            string productionId,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("RestoreProduction API triggered.");

            var restoreProductionInfo = new ProductionRequest()
            {
                ProductionStoreId = productionStoreId,
                ProductionId = productionId
            };
            var validator = new RestoreProductionValidator();
            var validationResult = validator.Validate(restoreProductionInfo);
            if (!validationResult.IsValid)
            {
                UnprocessableEntityObjectResult validationErrorMsg = new UnprocessableEntityObjectResult(validationResult.Errors.Select(e => new Response
                {
                    Success = false,
                    StatusCode = $"{Convert.ToInt32(HttpStatusCode.UnprocessableEntity)}-{HttpStatusCode.UnprocessableEntity}",
                    Error = new
                    {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage,
                    },
                    Data = null
                }));

                return validationErrorMsg;
            }

            var prodStore = await this.productionStoreRepository.GetProductionStoreById(productionStoreId);
            if (prodStore == null)
            {
                return new ObjectResult(new { id = string.Empty, name = string.Empty, error = $"No production store exists with: {productionStoreId} id." }) { StatusCode = (int)HttpStatusCode.NotFound };
            }

            var userAccess = this.baseSecurity.ValidateUserGroupsAccess(request, log, productionStoreId);
            userAccess.HasAccess = true;
            if (userAccess.HasAccess)
            {
                string instanceId = await starter.StartNewAsync("RestoreProductionOrchestrator", restoreProductionInfo);
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
                log.LogError("Access Forbidden to Restore Production API.");
                return new ObjectResult(new { id = prodStore.Id, name = prodStore.Name }) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
        }
    }
}