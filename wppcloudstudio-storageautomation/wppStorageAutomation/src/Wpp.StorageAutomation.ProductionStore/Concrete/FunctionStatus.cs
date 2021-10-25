// <copyright file="FunctionStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    ///  The function status.
    /// </summary>
    public class FunctionStatus : IFunctionStatus
    {
        /// <summary>Gets the function status.</summary>
        /// <param name="statusUri">The status URI.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<HttpResponseMessage> GetFunctionStatus(string statusUri, ILogger log)
        {
            log.LogInformation("getting function status.");
            HttpClient client = new HttpClient();

            var apiAppRegClientId = Environment.GetEnvironmentVariable("API_APPREG_CLIENT_ID");
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(apiAppRegClientId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await client.GetAsync(statusUri);
            return response;
        }
    }
}
