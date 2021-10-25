using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Wpp.StorageAutomation.ProductionControl
{
    /// <summary>
    /// Swagger Controller class.
    /// </summary>
    public static class SwaggerController
    {
        /// <summary>
        /// Runs the specified req.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="swashBuckleClient">The swash buckle client.</param>
        /// <returns>HttpResponseMessage.</returns>
        [SwaggerIgnore]
        [FunctionName("Swagger")]
        public static Task<HttpResponseMessage> Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pc/Swagger/json")] HttpRequestMessage req,
                [SwashBuckleClient] ISwashBuckleClient swashBuckleClient)
        {
            return Task.FromResult(swashBuckleClient.CreateSwaggerJsonDocumentResponse(req, "v1"));
        }

        /// <summary>
        /// Run2s the specified req.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="swashBuckleClient">The swash buckle client.</param>
        /// <returns>HttpResponseMessage.</returns>
        [SwaggerIgnore]
        [FunctionName("SwaggerUi")]
        public static Task<HttpResponseMessage> Run2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pc/Swagger/ui")] HttpRequestMessage req,
            [SwashBuckleClient] ISwashBuckleClient swashBuckleClient)
        {
            return Task.FromResult(swashBuckleClient.CreateSwaggerUIResponse(req, "pc/swagger/json"));
        }
    }
}
