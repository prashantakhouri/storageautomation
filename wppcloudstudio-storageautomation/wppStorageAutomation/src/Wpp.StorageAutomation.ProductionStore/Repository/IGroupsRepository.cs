// <copyright file="IGroupsRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Wpp.StorageAutomation.Entities.Models;

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    /// Group Repository.
    /// </summary>
    public interface IGroupsRepository
    {
        /// <summary>
        /// Adds the group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>A task.</returns>
        Task AddGroup(Groups group);

        /// <summary>
        /// Adds the group range.
        /// </summary>
        /// <param name="groupList">The group list.</param>
        /// <returns>A task.</returns>
        Task AddGroupRange(List<Groups> groupList);
    }
}