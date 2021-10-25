// <copyright file="IFunctionStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    ///  IFunctionStatus.
    /// </summary>
    public interface IFunctionStatus
    {
        /// <summary>Gets the function status.</summary>
        /// <param name="statusUri">The status URI.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        Task<HttpResponseMessage> GetFunctionStatus(string statusUri, ILogger log);
    }
}