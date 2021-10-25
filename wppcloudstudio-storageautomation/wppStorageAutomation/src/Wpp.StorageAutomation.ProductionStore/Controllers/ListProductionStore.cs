// <copyright file="ListProductionStore.cs" company="PlaceholderCompany">
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
using Wpp.StorageAutomation.Security;

namespace Wpp.StorageAutomation.ProductionStore.Controllers
{
    /// <summary>
    /// List production stores.
    /// </summary>
    public class ListProductionStore
    {
        private readonly IProductionStores productionStores;
        private readonly IBaseSecurity baseSecurity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListProductionStore"/> class.
        /// </summary>
        /// <param name="productionStores">The production stores.</param>
        /// <param name="baseSecurity">The base security.</param>
        public ListProductionStore(IProductionStores productionStores, IBaseSecurity baseSecurity)
        {
            this.productionStores = productionStores;
            this.baseSecurity = baseSecurity;
        }

        /// <summary>
        /// Lists the specified req.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="log">The log.</param>
        /// <returns>A response.</returns>
        [FunctionName("ListProductionStore")]
        public async Task<object> List(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "production-stores")] HttpRequest req,
           ILogger log)
        {
            try
            {
                log.LogInformation(" Production Store list API triggered.");
                var userAccess = this.baseSecurity.ValidateUserGroupsAccess(req, log, string.Empty);
                if (userAccess.HasAccess)
                {
                    if (userAccess.Groups.Any())
                    {
                        var outputs = new List<ProductionStoreListResponse>();
                        outputs.Add(await this.productionStores.ListProductionStoresAsync(userAccess.Groups));
                        return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.OK)}-{HttpStatusCode.OK}", Data = outputs, Error = null };
                    }
                    else
                    {
                        return new StatusCodeResult(403);
                    }
                }
                else
                {
                    log.LogError("Access Forbidden to Production Store list API.");
                    return new StatusCodeResult(403);
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error  while listing production stores: {ex.Message}");
                if (ex.Message.Contains("SqlException"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
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
