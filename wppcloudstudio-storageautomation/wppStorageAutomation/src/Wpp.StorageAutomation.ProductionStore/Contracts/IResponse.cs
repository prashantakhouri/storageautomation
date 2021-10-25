// <copyright file="IResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    /// Response.
    /// </summary>
    public interface IResponse
        {
            /// <summary>
            /// Gets or sets a value indicating whether gets or Sets Success.
            /// </summary>
            bool Success { get; set; }

            /// <summary>
            /// Gets a value indicating whether gets or Sets HttpStatusCode.
            /// </summary>
            string StatusCode { get; }

            /// <summary>
            /// Gets or sets a value indicating whether gets or Sets Error.
            /// </summary>
            object Error { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether gets or Sets Data.
            /// </summary>
            object Data { get; set; }
        }
    }
