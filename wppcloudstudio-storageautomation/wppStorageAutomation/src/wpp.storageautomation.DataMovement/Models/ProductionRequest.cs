// <copyright file="ProductionRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Wpp.StorageAutomation.DataMovement.Models
{
    /// <summary>
    ///   The production request.
    /// </summary>
    public class ProductionRequest
    {
        /// <summary>
        /// Gets or sets the production store identifier.
        /// </summary>
        /// <value>
        /// The production store identifier.
        /// </value>
        public string ProductionStoreId { get; set; }

        /// <summary>
        /// Gets or sets the production identifier.
        /// </summary>
        /// <value>
        /// The production identifier.
        /// </value>
        public string ProductionId { get; set; }
    }
}