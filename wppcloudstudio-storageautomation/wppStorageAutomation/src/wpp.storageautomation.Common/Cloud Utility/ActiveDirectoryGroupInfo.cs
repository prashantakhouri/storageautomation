// <copyright file="ActiveDirectoryGroupInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Wpp.StorageAutomation.Common
{
    /// <summary>
    /// Active Directory Group response.
    /// </summary>
    public class ActiveDirectoryGroupInfo
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the security identifier.
        /// </summary>
        /// <value>
        /// The security identifier.
        /// </value>
        public string SecurityIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the on premises security identifier.
        /// </summary>
        /// <value>
        /// The on premises security identifier.
        /// </value>
        public string OnPremisesSecurityIdentifier { get; set; }
    }
}
