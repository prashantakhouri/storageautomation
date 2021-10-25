// <copyright file="IGroupsRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Wpp.StorageAutomation.Entities.Models;

namespace Wpp.StorageAutomation.Security.Repository
{
    /// <summary>
    /// Groups Repository.
    /// </summary>
    public interface IGroupsRepository
    {
        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <param name="groupList">The group list.</param>
        /// <returns>A list of groups.</returns>
        List<Groups> GetGroups(List<string> groupList);
    }
}