// <copyright file="ArchiveAllResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;

namespace Wpp.StorageAutomation.DataMovement.Models
{
    /// <summary>
    ///   The archive all productions response.
    /// </summary>
    public class ArchiveAllResponse
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the restore date.
        /// </summary>
        /// <value>
        /// The restore date.
        /// </value>
        public DateTime? ArchiveDate { get; set; }
    }
}
