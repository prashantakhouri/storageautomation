// <copyright file="ProductionPropertiesInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Wpp.StorageAutomation.DataMovement.Models
{
    /// <summary>
    /// Details about empty directories, file properties will be strored in this format.
    /// </summary>
    public class ProductionPropertiesInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionPropertiesInfo"/> class.
        /// </summary>
        public ProductionPropertiesInfo()
        {
            this.Items = new Dictionary<string, PropertiesInfo>();
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IDictionary<string, PropertiesInfo> Items { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }
    }
}
