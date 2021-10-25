// <copyright file="ArchiveProductionStoreResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;

namespace Wpp.StorageAutomation.DataMovement.Models
{
    /// <summary>
    ///   The archive production response.
    /// </summary>
    public class ArchiveProductionStoreResponse
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

#nullable enable

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the restore date.
        /// </summary>
        /// <value>
        /// The restore date.
        /// </value>
        public DateTime? ArchiveDate { get; set; }
    }
}
