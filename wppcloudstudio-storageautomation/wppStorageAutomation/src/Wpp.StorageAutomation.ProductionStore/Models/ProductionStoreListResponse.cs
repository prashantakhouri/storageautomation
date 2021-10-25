// <copyright file="ProductionStoreListResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    /// The list production store response.
    /// </summary>
    public class ProductionStoreListResponse
    {
        /// <summary>
        /// Gets or sets the production list.
        /// </summary>
        /// <value>
        /// The production list.
        /// </value>
        public IEnumerable<ProductionStoreRow> ProductionStoreList { get; set; }
    }

    /// <summary>
    ///   The production store row in SQL DB.
    /// </summary>
    public class ProductionStoreRow
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary> Gets or sets the region.</summary>
        /// <value>The region.</value>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the wipurl.
        /// </summary>
        /// <value>
        /// The wipurl.
        /// </value>
        public string Wipurl { get; set; }

        /// <summary>
        /// Gets or sets the size of the wipallocated.
        /// </summary>
        /// <value>
        /// The size of the wipallocated.
        /// </value>
        public decimal? WipallocatedSize { get; set; }

        /// <summary>
        /// Gets or sets the archive URL.
        /// </summary>
        /// <value>
        /// The archive URL.
        /// </value>
        public string ArchiveUrl { get; set; }

        /// <summary>
        /// Gets or sets the size of the archive allocated.
        /// </summary>
        /// <value>
        /// The size of the archive allocated.
        /// </value>
        public decimal? ArchiveAllocatedSize { get; set; }

        /// <summary>
        /// Gets or sets the scale down time.
        /// </summary>
        /// <value>
        /// The scale down time.
        /// </value>
        public DateTime? ScaleDownTime { get; set; }

        /// <summary>
        /// Gets or sets the scale up time interval.
        /// </summary>
        /// <value>
        /// The scale up time interval.
        /// </value>
        public DateTime? ScaleUpTimeInterval { get; set; }

        /// <summary>
        /// Gets or sets the minimum size of the free.
        /// </summary>
        /// <value>
        /// The minimum size of the free.
        /// </value>
        public decimal? MinimumFreeSize { get; set; }

        /// <summary>
        /// Gets or sets the minimum free space.
        /// </summary>
        /// <value>
        /// The minimum free space.
        /// </value>
        public decimal? MinimumFreeSpace { get; set; }

        /// <summary>
        /// Gets or sets the offline time.
        /// </summary>
        /// <value>
        /// The offline time.
        /// </value>
        public string OfflineTime { get; set; }

        /// <summary>
        /// Gets or sets the online time.
        /// </summary>
        /// <value>
        /// The online time.
        /// </value>
        public string OnlineTime { get; set; }

        /// <summary>
        /// Gets or sets the production offline time interval.
        /// </summary>
        /// <value>
        /// The production offline time interval.
        /// </value>
        public decimal? ProductionOfflineTimeInterval { get; set; }

        /// <summary>
        /// Gets or sets the group names.
        /// </summary>
        /// <value>
        /// The group names.
        /// </value>
        public string ManagerRoleGroupNames { get; set; }

        /// <summary>
        /// Gets or sets the group names.
        /// </summary>
        /// <value>
        /// The group names.
        /// </value>
        public string UserRoleGroupNames { get; set; }

        /// <summary>
        /// Gets or sets the name of the wipkey.
        /// </summary>
        /// <value>
        /// The name of the wipkey.
        /// </value>
        public string WipkeyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the archive key.
        /// </summary>
        /// <value>
        /// The name of the archive key.
        /// </value>
        public string ArchiveKeyName { get; set; }
    }
}
