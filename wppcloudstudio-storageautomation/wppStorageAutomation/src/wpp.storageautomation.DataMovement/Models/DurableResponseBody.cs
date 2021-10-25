// <copyright file="DurableResponseBody.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Wpp.StorageAutomation.DataMovement.Models
{
    /// <summary>
    /// Response object to get status query Uri.
    /// </summary>
    public class DurableResponseBody
    {
        /// <summary>
        /// Gets or sets the status query get URI.
        /// </summary>
        /// <value>
        /// The status query get URI.
        /// </value>
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public string statusQueryGetUri { get; set; }
#pragma warning restore SA1300 // Element should begin with upper-case letter
    }
}
