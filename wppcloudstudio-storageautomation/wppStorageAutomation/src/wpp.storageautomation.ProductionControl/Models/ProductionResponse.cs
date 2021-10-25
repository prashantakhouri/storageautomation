// <copyright file="ProductionResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;

namespace Wpp.StorageAutomation
{
    /// <summary>
    ///   The production response.
    /// </summary>
    public class ProductionResponse
    {
        /// <summary>Gets or sets the production store URI.</summary>
        /// <value>The production store URI.</value>
        public string Id { get; set; }

        /// <summary>Gets or sets the name of the production.</summary>
        /// <value>The name of the production.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the WIP URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Wipurl { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the last synchronize date time.
        /// </summary>
        /// <value>The last synchronize date time.</value>
        public DateTime? LastSyncDateTime { get; set; }
    }
}