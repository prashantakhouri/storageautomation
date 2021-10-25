// <copyright file="GetProduction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation.Production
{
    /// <summary>
    ///   The get production controller.
    /// </summary>
    public class GetProduction
    {
        private readonly IProduction prod;
        private readonly IBaseSecurity baseSecurity;
        private readonly IProductionStoreRepository productionStoreRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetProduction"/> class.
        /// </summary>
        /// <param name="prod">The product.</param>
        /// <param name="baseSecurity">The base security.</param>
        /// <param name="productionStoreRepository">The production store repository.</param>
        public GetProduction(IProduction prod, IBaseSecurity baseSecurity, IProductionStoreRepository productionStoreRepository)
        {
            this.prod = prod;
            this.baseSecurity = baseSecurity;
            this.productionStoreRepository = productionStoreRepository;
        }

        /// <summary>Gets the GetProductionStarter starter.</summary>
        /// <param name="req">The req.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <param name="log">The logger.</param>
        /// <returns>
        ///   A list of production information.
        /// </returns>
        [FunctionName("GetProduction")]
        public async Task<object> Get(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "production-stores/{productionStoreId}/productions")] HttpRequest req, string productionStoreId, ILogger log)
        {
            try
            {
                log.LogInformation("GetProduction API triggered.");

                var prodStore = await this.productionStoreRepository.GetProductionStoreById(productionStoreId);
                if (prodStore == null)
                {
                    return new ObjectResult(new { id = string.Empty, name = string.Empty, error = $"No production store exists with: {productionStoreId} id." }) { StatusCode = (int)HttpStatusCode.NotFound };
                }

                var userAccess = this.baseSecurity.ValidateUserGroupsAccess(req, log, productionStoreId);
                if (userAccess.HasAccess)
                {
                    log.LogInformation("GetProduction API triggered.");
                    var outputs = new List<ProductionListResponse>();
                    outputs.Add(await this.prod.GetProductionsByProductionStoreAsync(productionStoreId, userAccess.Groups));
                    return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.OK)}-{HttpStatusCode.OK}", Data = outputs, Error = null };
                }
                else
                {
                    log.LogError("Access forbidden to GetProduction API.");
                    return new ObjectResult(new { id = prodStore.Id, name = prodStore.Name }) { StatusCode = (int)HttpStatusCode.Forbidden };
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error while getting productions : {ex.Message}");
                if (ex.Message.Contains("SqlException"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError.ToString()}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("Production store not found"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.NotFound)}-{HttpStatusCode.NotFound.ToString()}", Data = null, Error = ex.Message };
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
