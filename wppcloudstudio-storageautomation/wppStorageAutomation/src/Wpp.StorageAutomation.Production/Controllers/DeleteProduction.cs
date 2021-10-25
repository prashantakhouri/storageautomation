// <copyright file="DeleteProduction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation.Production.Controllers
{
    /// <summary>
    /// Delete production.
    /// </summary>
    public class DeleteProduction
    {
        private readonly IProduction prod;
        private readonly IProductionStoreRepository productionStoreRepository;
        private readonly IBaseSecurity baseSecurity;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteProduction"/> class.
        /// </summary>
        /// <param name="prod">The product.</param>
        /// <param name="baseSecurity">The base security.</param>
        /// <param name="productionStoreRepository">The production store repository.</param>
        public DeleteProduction(IProduction prod, IBaseSecurity baseSecurity, IProductionStoreRepository productionStoreRepository)
        {
            this.prod = prod;
            this.baseSecurity = baseSecurity;
            this.productionStoreRepository = productionStoreRepository;
        }

        /// <summary>Deletes the production.</summary>
        /// <param name="req">The req.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="productionId">The production identifier.</param>
        /// <param name="log">The logger.</param>
        /// <returns>
        ///   A list of production information.
        /// </returns>
        [FunctionName("DeleteProduction")]
        public async Task<object> Delete(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "production-stores/{productionStoreId}/productions/{productionId}")] HttpRequest req, string productionStoreId, string productionId, ILogger log)
        {
            try
            {
                log.LogInformation("DeleteProduction API triggered.");

                var prodStore = await this.productionStoreRepository.GetProductionStoreById(productionStoreId);
                if (prodStore == null)
                {
                    return new ObjectResult(new { id = string.Empty, name = string.Empty, error = $"No production store exists with: {productionStoreId} id." }) { StatusCode = (int)HttpStatusCode.NotFound };
                }

                var userAccess = this.baseSecurity.ValidateManagerGroupsAccesstoStores(req, log, productionStoreId);
                if (userAccess.HasAccess)
                {
                    log.LogInformation("DeleteProduction API triggered.");
                    var outputs = new List<ProductionResponse>();
                    outputs.Add(await this.prod.DeleteProduction(productionStoreId, productionId));
                    return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.OK)}-{HttpStatusCode.OK}", Data = outputs, Error = null };
                }
                else
                {
                    log.LogError("Access forbidden to DeleteProduction API.");
                    return new ObjectResult(new { id = prodStore.Id, name = prodStore.Name }) { StatusCode = (int)HttpStatusCode.Forbidden };
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error  while soft deleting production : {ex.Message}");
                if (ex.Message.Contains("SqlException"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError.ToString()}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("entry was not found"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.NotFound)}-{HttpStatusCode.NotFound.ToString()}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("since the status is online") || ex.Message.Contains("already deleted"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.BadRequest)}-{HttpStatusCode.BadRequest}", Data = null, Error = ex.Message };
                }

                if (ex.Message.ToLower().Contains("timeout"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.RequestTimeout)}-{HttpStatusCode.RequestTimeout.ToString()}", Data = null, Error = ex.Message };
                }

                return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError.ToString()}", Data = null, Error = ex.Message };
            }
        }
    }
}