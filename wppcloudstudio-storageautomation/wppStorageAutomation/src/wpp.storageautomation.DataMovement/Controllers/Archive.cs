// <copyright file="Archive.cs" company="PlaceholderCompany">
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
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.DataMovement.Models;
using Wpp.StorageAutomation.DataMovement.Models.Validators;
using Wpp.StorageAutomation.DataMovement.Repository;

namespace Wpp.StorageAutomation.DataMovement
{
    /// <summary>
    /// Archive.
    /// </summary>
    public class Archive
    {
        private readonly IProductionStoreRepository productionStoreRepository;
        private readonly IProductionRepository productionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Archive" /> class.
        /// </summary>
        /// <param name="productionStoreRepository">The production store repository.</param>
        /// <param name="productionRepository">The production repository.</param>
        public Archive(IProductionStoreRepository productionStoreRepository, IProductionRepository productionRepository)
        {
            this.productionStoreRepository = productionStoreRepository;
            this.productionRepository = productionRepository;
        }

        /// <summary>Archive production starter.</summary>
        /// <param name="request">The request.</param>
        /// <param name="starter">The starter.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        ///   A http response message.
        /// </returns>
        [FunctionName(nameof(Archive))]
        public async Task<IActionResult> ArchiveAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "production-stores/archive")] HttpRequest request,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("ArchiveOrchestrator", null);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'. \n");

            var response = starter.CreateCheckStatusResponse(request, instanceId);
            var okObjectResult = response as ObjectResult;
            var value = okObjectResult.Value as HttpResponseMessage;
            var content = await value.Content.ReadAsStringAsync();
            var body = JsonSerializer.Deserialize<DurableResponseBody>(content);

            var funcStatusUri = new Uri(body.statusQueryGetUri);
            var appendPath = funcStatusUri.ToString().Split("instances/", StringSplitOptions.RemoveEmptyEntries)[1];
            string apimBaseUri = Environment.GetEnvironmentVariable("APIM_BASE_URL");
            var apimGetStatusQuery = apimBaseUri + appendPath;

            await this.productionRepository.UpdateArchiveGetStatusQueryUri(apimGetStatusQuery);

            return starter.CreateCheckStatusResponse(request, instanceId);
        }

        /// <summary>
        /// Archives the production store starter.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="starter">The starter.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        /// A http response message.
        /// </returns>
        [FunctionName(nameof(ArchiveProductionStore))]
        public async Task<IActionResult> ArchiveProductionStore(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "production-stores/{productionStoreId}/productions/archive")] HttpRequest request,
            string productionStoreId,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var validator = new ProductionStoreIdValidator();
            var validationResult = validator.Validate(productionStoreId);
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

            string instanceId = await starter.StartNewAsync<string>("ArchiveProductionStoreOrchestrator", productionStoreId);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'. \n");

            var response = starter.CreateCheckStatusResponse(request, instanceId);
            var okObjectResult = response as ObjectResult;
            var value = okObjectResult.Value as HttpResponseMessage;
            var content = await value.Content.ReadAsStringAsync();
            var body = JsonSerializer.Deserialize<DurableResponseBody>(content);

            var funcStatusUri = new Uri(body.statusQueryGetUri);
            var appendPath = funcStatusUri.ToString().Split("instances/", StringSplitOptions.RemoveEmptyEntries)[1];
            string apimBaseUri = Environment.GetEnvironmentVariable("APIM_BASE_URL");
            var apimGetStatusQuery = apimBaseUri + appendPath;

            await this.productionRepository.UpdateArchiveGetStatusQueryUri(productionStoreId, apimGetStatusQuery);

            return starter.CreateCheckStatusResponse(request, instanceId);
        }

        /// <summary>
        /// Archives the production starter.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="productionId">The production identifier.</param>
        /// <param name="starter">The starter.</param>
        /// <param name="log">The log.</param>
        /// <returns>The response message.</returns>
        [FunctionName(nameof(ArchiveProduction))]
        public async Task<IActionResult> ArchiveProduction(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "production-stores/{productionStoreId}/productions/{productionId}/archive")] HttpRequest request,
            string productionStoreId,
            string productionId,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var archiveProductionRequest = new ProductionRequest()
            {
                ProductionStoreId = productionStoreId,
                ProductionId = productionId,
            };

            var validator = new ArchiveProductionValidator();
            var validationResult = validator.Validate(archiveProductionRequest);
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

            string instanceId = await starter.StartNewAsync<ProductionRequest>("ArchiveProductionOrchestrator", archiveProductionRequest);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'. \n");

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
    }
}