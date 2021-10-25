// <copyright file="ProductionStoreResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    /// Production store response.
    /// </summary>
    public class ProductionStoreResponse
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the CreatedDate.
        /// </summary>
        /// <value>
        /// The createdDate.
        /// </value>
        public DateTime CreatedDate { get; set; }
    }
}
