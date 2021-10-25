// <copyright file="IBaseSecurity.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Wpp.StorageAutomation.Security
{
    /// <summary>
    /// Security interface.
    /// </summary>
    public interface IBaseSecurity
    {
        /// <summary>
        /// Gets the user groups.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="log">The log.</param>
        /// <returns>Returns UserGroupResponse.</returns>
        UserGroupResponse GetUserGroups(HttpRequest req, ILogger log);

        /// <summary>
        /// Validates the user groups access.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="log">The log.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>Returns UserGroupResponse.</returns>
        UserGroupResponse ValidateUserGroupsAccess(HttpRequest req, ILogger log, string productionStoreId);

        /// <summary>
        /// Validates the user groups access.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <param name="log">The log.</param>
        /// <param name="productionStoreId">The production store identifier.</param>
        /// <returns>Returns UserGroupResponse.</returns>
        UserGroupResponse ValidateManagerGroupsAccesstoStores(HttpRequest req, ILogger log, string productionStoreId);
    }
}