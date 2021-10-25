// <copyright file="OrchestratorFunctions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Wpp.StorageAutomation.ProductionControl
{
    /// <summary>
    ///   The orchestrator functions class.
    /// </summary>
    public static class OrchestratorFunctions
    {
        /// <summary>
        /// Gets the production orchestrator.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [FunctionName(nameof(GetProductionOrchestrator))]
        public static async Task<object> GetProductionOrchestrator(
           [OrchestrationTrigger] IDurableOrchestrationContext context,
           ILogger log)
        {
            try
            {
                string productionStoreId = context.GetInput<string>();

                var outputs = new List<ProductionListResponse>();
                outputs.Add(await context.CallActivityAsync<ProductionListResponse>("GetProductionsByProductionStoreAsync", productionStoreId));

                return new Response { Success = true, StatusCode = $"{HttpStatusCode.OK}-{HttpStatusCode.OK.ToString()}", Data = outputs, Error = null };
            }
            catch (Exception ex)
            {
                log.LogError($"Error  while getting productions : {ex.Message}");
                if (ex.Message.Contains("SqlException"))
                {
                    return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError.ToString()}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("Production store not found"))
                {
                    return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.NotFound)}-{HttpStatusCode.NotFound.ToString()}", Data = null, Error = ex.Message };
                }

                if (ex.Message.ToLower().Contains("timeout"))
                {
                    return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.RequestTimeout)}-{HttpStatusCode.RequestTimeout.ToString()}", Data = null, Error = ex.Message };
                }

                return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError.ToString()}", Data = null, Error = ex.Message };
            }
        }

        /// <summary>
        /// Creates the production orchestrator.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [FunctionName(nameof(CreateProductionOrchestrator))]
        public static async Task<object> CreateProductionOrchestrator(
           [OrchestrationTrigger] IDurableOrchestrationContext context,
           ILogger log)
        {
            try
            {
                ProductionRequest productionRequest = context.GetInput<ProductionRequest>();
                var outputs = new List<ProductionResponse>();
                log.LogInformation("Call to CreateProduction activity");
                outputs.Add(await context.CallActivityAsync<ProductionResponse>("CreateProduction", productionRequest));

                return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.OK)}-{HttpStatusCode.OK.ToString()}", Data = outputs, Error = null };
            }
            catch (Exception ex)
            {
                log.LogError($"Error  while creating a production: {ex.Message}");
                if (ex.Message.Contains("SqlException"))
                {
                    return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError.ToString()}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("Production store not found"))
                {
                    return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.NotFound)}-{HttpStatusCode.NotFound.ToString()}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("Duplicate"))
                {
                    return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.UnprocessableEntity)}-{HttpStatusCode.UnprocessableEntity.ToString()}", Data = null, Error = ex.Message };
                }

                if (ex.Message.ToLower().Contains("timeout"))
                {
                    return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.RequestTimeout)}-{HttpStatusCode.RequestTimeout.ToString()}", Data = null, Error = ex.Message };
                }

                return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError.ToString()}", Data = null, Error = ex.Message };
            }
        }

        /// <summary>Deletes the production orchestrator.</summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        [FunctionName(nameof(DeleteProductionOrchestrator))]
        public static async Task<List<string>> DeleteProductionOrchestrator(
           [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            outputs.Add(await context.CallActivityAsync<string>("DeleteProduction", "Tokyo"));

            return outputs;
        }
    }
}