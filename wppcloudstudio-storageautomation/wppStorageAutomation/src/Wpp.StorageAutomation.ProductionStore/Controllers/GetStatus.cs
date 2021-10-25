// <copyright file="GetStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Wpp.StorageAutomation.ProductionStore.Controllers
{
    /// <summary>
    ///   The get status controller.
    /// </summary>
    public class GetStatus
    {
        private readonly IFunctionStatus functionStatus;

        /// <summary>Initializes a new instance of the <see cref="GetStatus" /> class.</summary>
        /// <param name="functionStatus">The function status.</param>
        public GetStatus(IFunctionStatus functionStatus)
        {
            this.functionStatus = functionStatus;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="instanceid">The instance id.</param>
        /// <param name="log">The log.</param>
        /// <returns>A response.</returns>
        [FunctionName("GetFunctionStatus")]
        public async Task<object> List(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getfunctionstatus/{instanceid}")] HttpRequest req, string instanceid, ILogger log)
        {
            try
            {
                log.LogInformation("Get status API triggered.");
                string baseUri = Environment.GetEnvironmentVariable("API_BASE_URL");
                string funcUri = baseUri + "/runtime/webhooks/durabletask/instances/" + instanceid + req.QueryString.Value;
                var outputs = await this.functionStatus.GetFunctionStatus(funcUri, log);
                return outputs;
            }
            catch (Exception ex)
            {
                log.LogError($"Error  while getting function status: {ex.Message}");
                if (ex.Message.Contains("not found"))
                {
                    return new Response
                    {
                        Success = false,
                        StatusCode = $"{Convert.ToInt32(HttpStatusCode.NotFound)}-{HttpStatusCode.NotFound}",
                        Data = null,
                        Error = ex.Message
                    };
                }

                return new Response { Success = false, StatusCode = $"{Convert.ToInt32(HttpStatusCode.InternalServerError)}-{HttpStatusCode.InternalServerError.ToString()}", Data = null, Error = ex.Message };
            }
        }
    }
}
