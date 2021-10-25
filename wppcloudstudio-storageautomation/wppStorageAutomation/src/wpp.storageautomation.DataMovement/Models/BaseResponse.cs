// <copyright file="BaseResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Wpp.StorageAutomation.DataMovement.Models
{
    /// <summary>
    ///   The base response.
    /// </summary>
    public class BaseResponse
    {
        /// <summary>Gets or sets the message.</summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>Gets or sets the code.</summary>
        /// <value>The code.</value>
        public int Code { get; set; }
    }
}
