// <copyright file="RegisterProductionStore.cs" company="PlaceholderCompany">
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

namespace Wpp.StorageAutomation.ProductionStore.Controllers
{
    /// <summary>
    /// Register productions tore.
    /// </summary>
    public class RegisterProductionStore
    {
        private readonly IProductionStores productionStores;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterProductionStore"/> class.
        /// </summary>
        /// <param name="productionStores">The production stores.</param>
        public RegisterProductionStore(IProductionStores productionStores)
        {
            this.productionStores = productionStores;
        }

        /// <summary>
        /// Registers the specified req.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="log">The log.</param>
        /// <returns>A response.</returns>
        [FunctionName("RegisterProductionStore")]
        public async Task<object> Register(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "production-stores/")] HttpRequest req,
           ILogger log)
        {
            try
            {
                    var jsonReq = await req.ReadAsStringAsync();
                    ProductionStoreRequest data = JsonConvert.DeserializeObject<ProductionStoreRequest>(jsonReq);
                    log.LogInformation("Register a production store api called.");
                    var validator = new ProductionStoreRequestValidator();
                    var validationResult = validator.Validate(data);
                    if (!validationResult.IsValid)
                    {
                        UnprocessableEntityObjectResult validationErrorMsg = new UnprocessableEntityObjectResult(validationResult.Errors.Select(e => new
                        {
                            Field = e.PropertyName,
                            Error = e.ErrorMessage,
                        }));
                        Response response = new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.UnprocessableEntity)}-{HttpStatusCode.UnprocessableEntity}", Data = null, Error = validationErrorMsg.Value };
                        return new UnprocessableEntityObjectResult(response);
                    }

                    var outputs = new List<ProductionStoreResponse>();
                    outputs.Add(await this.productionStores.SaveProductionStore(data));
                    return new Response { Success = true, StatusCode = $"{Convert.ToInt32(HttpStatusCode.OK)}-{HttpStatusCode.OK}", Data = outputs, Error = null };
            }
            catch (Exception ex)
            {
                log.LogError($"Error  while registering a production store: {ex.Message}");
                if (ex.Message.Contains("SqlException"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("Production store record already exists"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.UnprocessableEntity)}-{HttpStatusCode.UnprocessableEntity}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("does not exist in the WIP"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.UnprocessableEntity)}-{HttpStatusCode.UnprocessableEntity}", Data = null, Error = ex.Message };
                }

                if (ex.Message.ToLower().Contains("timeout"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.RequestTimeout)}-{HttpStatusCode.RequestTimeout}", Data = null, Error = ex.Message };
                }

                if (ex.Message.Contains("count does not match"))
                {
                    return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.UnprocessableEntity)}-{HttpStatusCode.UnprocessableEntity}", Data = null, Error = ex.Message };
                }

                return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError}", Data = null, Error = ex.Message };
            }
        }
    }
}
