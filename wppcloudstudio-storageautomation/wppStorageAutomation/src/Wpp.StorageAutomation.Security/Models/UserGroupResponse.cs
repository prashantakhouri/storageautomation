// <copyright file="UserGroupResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Wpp.StorageAutomation.Security
{
    /// <summary>
    /// UserGroupResponse.
    /// </summary>
    public class UserGroupResponse
    {
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has access.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has access; otherwise, <c>false</c>.
        /// </value>
        public bool HasAccess { get; set; }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        public List<string> Groups { get; set; }
    }
}
