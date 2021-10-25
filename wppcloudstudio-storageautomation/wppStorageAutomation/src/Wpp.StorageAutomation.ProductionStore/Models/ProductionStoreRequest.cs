// <copyright file="ProductionStoreRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    /// Gets  the production store request.
    /// </summary>
    public class ProductionStoreRequest
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the WIPURL.
        /// </summary>
        /// <value>
        /// The WIPURL.
        /// </value>
        public string WIPURL { get; set; }

        /// <summary>
        /// Gets or sets the WIPAllocatedSize.
        /// </summary>
        /// <value>
        /// The WIPAllocatedSize.
        /// </value>
        public decimal? WIPAllocatedSize { get; set; }

        /// <summary>
        /// Gets or sets the archiveURL.
        /// </summary>
        /// <value>
        /// The archiveURL.
        /// </value>
        public string ArchiveURL { get; set; }

        /// <summary>
        /// Gets or sets the archiveAllocatedSize.
        /// </summary>
        /// <value>
        /// The archiveAllocatedSize.
        /// </value>
        public decimal? ArchiveAllocatedSize { get; set; }

        /// <summary>
        /// Gets or sets the UserRoleGroupNames.
        /// </summary>
        /// <value>
        /// The UserRoleGroupNames.
        /// </value>
        public string UserRoleGroupNames { get; set; }

        /// <summary>
        /// Gets or sets the ManagerRoleGroupNames.
        /// </summary>
        /// <value>
        /// The ManagerRoleGroupNames.
        /// </value>
        public string ManagerRoleGroupNames { get; set; }

        /// <summary>
        /// Gets or sets the UserRoleGroupSIDs.
        /// </summary>
        /// <value>
        /// The UserRoleGroupSIDs.
        /// </value>
        public string UserRoleGroupSIDs { get; set; }

        /// <summary>
        /// Gets or sets the ManagerRoleGroupSIDs.
        /// </summary>
        /// <value>
        /// The ManagerRoleGroupSIDs.
        /// </value>
        public string ManagerRoleGroupSIDs { get; set; }

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
