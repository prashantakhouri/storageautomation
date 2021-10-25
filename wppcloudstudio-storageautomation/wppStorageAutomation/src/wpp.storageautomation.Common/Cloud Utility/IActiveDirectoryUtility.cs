// <copyright file="IActiveDirectoryUtility.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Wpp.StorageAutomation.Common
{
    /// <summary>
    /// Active Directory Utility.
    /// </summary>
    public interface IActiveDirectoryUtility
    {
        /// <summary>
        /// Ads the group details from graph API.
        /// </summary>
        /// <param name="groupsList">The groups.</param>
        /// <returns>List.</returns>
        public List<ActiveDirectoryGroupInfo> AdGroupDetailsFromGraphApi(List<string> groupsList);
    }
}
