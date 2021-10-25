// <copyright file="OrchestratorFunctions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Wpp.StorageAutomation.DataMovement.Contracts;
using Wpp.StorageAutomation.DataMovement.Models;

namespace Wpp.StorageAutomation.DataMovement
{
    /// <summary>
    /// The orchestrator functions class.
    /// </summary>
    public class OrchestratorFunctions
    {
        private readonly IDataMovement dataMovement;
        private readonly int timeoutInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestratorFunctions"/> class.
        /// </summary>
        /// <param name="dataMovement">The data movement.</param>
        public OrchestratorFunctions(IDataMovement dataMovement)
        {
            this.dataMovement = dataMovement;
            this.timeoutInterval = Convert.ToInt32(Environment.GetEnvironmentVariable("DurableFunctionTimeoutInterval"));
        }

        /// <summary>
        /// The archive production orchestrator.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        /// The response.
        /// </returns>
        [FunctionName(nameof(ArchiveOrchestrator))]
        public static async Task<object> ArchiveOrchestrator(
           [OrchestrationTrigger] IDurableOrchestrationContext context,
           ILogger log)
        {
            try
            {
                var outputs = new List<ArchiveAllResponse>();
                outputs.Add(await context.CallActivityAsync<ArchiveAllResponse>("ArchiveAsync", null));
                return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.OK)}-{HttpStatusCode.OK}", Data = outputs, Error = null };
            }
            catch (Exception ex)
            {
                log.LogError($"Error  while archiving : {ex.Message}");

                if (ex.Message.Contains("SqlException"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
                }

                if (ex.Message.ToLower().Contains("timeout"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.RequestTimeout)}-{HttpStatusCode.RequestTimeout}", Data = null, Error = ex.Message };
                }

                return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
            }
        }

        /// <summary>
        /// Archives the production orchestrator.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        /// The response.
        /// </returns>
        [FunctionName(nameof(ArchiveProductionOrchestrator))]
        public static async Task<object> ArchiveProductionOrchestrator(
                   [OrchestrationTrigger] IDurableOrchestrationContext context,
                   ILogger log)
        {
            try
            {
                var archiveProductionRequest = context.GetInput<ProductionRequest>();
                var outputs = new List<ArchiveProductionResponse>();
                outputs.Add(await context.CallActivityAsync<ArchiveProductionResponse>("ArchiveProductionAsync", archiveProductionRequest));
                return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.OK)}-{HttpStatusCode.OK}", Data = outputs };
            }
            catch (Exception ex)
            {
                log.LogError($"Error  while archiving a production : {ex.Message}");
                if (ex.Message.Contains("does not exist in WIP"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.NotFound)}-{HttpStatusCode.NotFound}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("SqlException"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
                }

                if (ex.Message.ToLower().Contains("timeout"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.RequestTimeout)}-{HttpStatusCode.RequestTimeout}", Data = null, Error = ex.Message };
                }

                return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
            }
        }

        /// <summary>
        /// The restore production orchestrator.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        /// The response.
        /// </returns>
        [FunctionName(nameof(RestoreProductionOrchestrator))]
        public static async Task<object> RestoreProductionOrchestrator(
           [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            try
            {
                ProductionRequest restoreProductionInfo = context.GetInput<ProductionRequest>();
                var outputs = new List<ProductionResponse>();
                outputs.Add(await context.CallActivityAsync<ProductionResponse>("RestoreProductionAsync", restoreProductionInfo));
                return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.OK)}-{HttpStatusCode.OK}", Data = outputs };
            }
            catch (Exception ex)
            {
                log.LogError($"Error  while restoring a production: {ex.Message}");
                if (ex.Message.Contains("does not exist") || ex.Message.Contains("entry was not found") || ex.Message.Contains("not found in key vault"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.NotFound)}-{HttpStatusCode.NotFound}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("already exists in WIP") || ex.Message.Contains("status is online"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.BadRequest)}-{HttpStatusCode.BadRequest}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("was not found in Archive. As a result, created date and updated date didn't get preserved. Empty directories are lost."))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.PartialContent)}-{HttpStatusCode.PartialContent}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("SqlException"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
                }

                if (ex.Message.ToLower().Contains("timeout"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.RequestTimeout)}-{HttpStatusCode.RequestTimeout}", Data = null, Error = ex.Message };
                }

                return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
            }
        }

        /// <summary>
        /// The restore production orchestrator.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        /// The response.
        /// </returns>
        [FunctionName(nameof(MakeProductionOfflineOrchestrator))]
        public static async Task<object> MakeProductionOfflineOrchestrator(
           [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            try
            {
                ProductionRequest productionInfo = context.GetInput<ProductionRequest>();
                var outputs = new List<ProductionResponse>();
                outputs.Add(await context.CallActivityAsync<ProductionResponse>("MakeProductionOfflineAsync", productionInfo));
                return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.OK)}-{HttpStatusCode.OK}", Data = outputs };
            }
            catch (Exception ex)
            {
                log.LogError($"Error  while making a production offline: {ex.Message}");
                if (ex.Message.Contains("does not exist") || ex.Message.Contains("entry was not found") || ex.Message.Contains("not found in key vault"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.NotFound)}-{HttpStatusCode.NotFound}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("already offline") || ex.Message.Contains("with open files or folders"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.BadRequest)}-{HttpStatusCode.BadRequest}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("SqlException"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
                }

                if (ex.Message.ToLower().Contains("timeout"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.RequestTimeout)}-{HttpStatusCode.RequestTimeout}", Data = null, Error = ex.Message };
                }

                return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
            }
        }

        /// <summary>
        /// Archives the production store orchestrator.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        /// The response.
        /// </returns>
        [FunctionName(nameof(ArchiveProductionStoreOrchestrator))]
        public async Task<object> ArchiveProductionStoreOrchestrator(
           [OrchestrationTrigger] IDurableOrchestrationContext context,
           ILogger log)
        {
            string productionStoreId = context.GetInput<string>();
            var outputs = new List<ArchiveProductionStoreResponse>();

            TimeSpan timeout = TimeSpan.FromMinutes(this.timeoutInterval);
            DateTime deadline = context.CurrentUtcDateTime.Add(timeout);

            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    Task<ArchiveProductionStoreResponse> archiveTask = this.dataMovement.ArchiveProductionStoreAsync(productionStoreId);
                    Task timeoutTask = context.CreateTimer(deadline, cts.Token);

                    Task winner = await Task.WhenAny(archiveTask, timeoutTask);
                    if (winner == archiveTask)
                    {
                        // success case
                        cts.Cancel();
                        outputs.Add(archiveTask.Result);
                        return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.OK)}-{HttpStatusCode.OK}", Data = outputs, Error = null };
                    }
                    else
                    {
                        // timeout case
                        return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.RequestTimeout)}-{HttpStatusCode.RequestTimeout}", Data = null, Error = "Durable function has timed out." };
                    }
                }
                catch (Exception ex)
                {
                    log.LogError($"Error  while archiving a production store : {ex.Message}");
                    if (ex.Message.Contains("does not exist in WIP") || ex.Message.Contains("entry was not found") || ex.Message.Contains("not found in key vault"))
                    {
                        return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.NotFound)}-{HttpStatusCode.NotFound}", Data = null, Error = ex.Message };
                    }

                    if (ex.Message.Contains("SqlException"))
                    {
                        return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
                    }

                    if (ex.Message.ToLower().Contains("timeout"))
                    {
                        return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.RequestTimeout)}-{HttpStatusCode.RequestTimeout}", Data = null, Error = ex.Message };
                    }

                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
                }
            }
        }
    }
}
