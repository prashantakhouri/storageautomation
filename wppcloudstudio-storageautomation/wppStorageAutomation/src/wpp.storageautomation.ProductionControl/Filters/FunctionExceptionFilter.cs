// <copyright file="FunctionExceptionFilter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Wpp.StorageAutomation.ProductionControl
{
    /// <summary>
    /// Class FunctionExceptionFilter.
    /// Implements the <see cref="Microsoft.Azure.WebJobs.Host.IFunctionExceptionFilter" />.
    /// </summary>
    /// <seealso cref="Microsoft.Azure.WebJobs.Host.IFunctionExceptionFilter" />
    public class FunctionExceptionFilter : IFunctionExceptionFilter
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionExceptionFilter"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public FunctionExceptionFilter(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Called when [exception asynchronous].
        /// </summary>
        /// <param name="exceptionContext">The exception context.</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        public Task OnExceptionAsync(FunctionExceptionContext exceptionContext, CancellationToken cancellationToken)
        {
            exceptionContext.Logger.LogInformation($"---- LOG: Exception in function {exceptionContext.FunctionName} ----");
            return Task.CompletedTask;
        }
    }
}
