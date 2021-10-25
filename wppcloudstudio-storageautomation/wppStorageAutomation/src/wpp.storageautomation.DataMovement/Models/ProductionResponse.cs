// <copyright file="ProductionResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;

namespace Wpp.StorageAutomation.DataMovement.Models
{
    /// <summary>The restore/offline production response.</summary>
    public class ProductionResponse
    {
        /// <summary>Gets or sets the production store URI.</summary>
        /// <value>The production store URI.</value>
        public string Id { get; set; }

        /// <summary>Gets or sets the name of the production.</summary>
        /// <value>The name of the production.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the modified date time.
        /// </summary>
        /// <value>The modified date time.</value>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public long? SizeInBytes { get; set; }
    }
}