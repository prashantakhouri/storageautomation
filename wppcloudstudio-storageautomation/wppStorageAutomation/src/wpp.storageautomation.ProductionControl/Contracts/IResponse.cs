using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Wpp.StorageAutomation.ProductionControl
{
    /// <summary>
    /// <see cref="IResponse"/>.
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
