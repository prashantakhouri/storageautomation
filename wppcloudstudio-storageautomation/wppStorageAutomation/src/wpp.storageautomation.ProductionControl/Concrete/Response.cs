// <copyright file="Response.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Wpp.StorageAutomation.ProductionControl
{
    /// <summary>
    /// Response.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.ProductionControl.IResponse" />
    public class Response : IResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether gets or Sets Success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        /// <value>
        /// The HTTP status code.
        /// </value>
        public string StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or Sets Error.
        /// </summary>
        public object Error { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or Sets Data.
        /// </summary>
        public object Data { get; set; }
    }
}
