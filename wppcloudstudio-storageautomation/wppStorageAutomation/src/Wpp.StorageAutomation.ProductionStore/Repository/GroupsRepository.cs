// <copyright file="GroupsRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wpp.StorageAutomation.Entities.Models;

namespace Wpp.StorageAutomation.ProductionStore
{
    /// <summary>
    /// Groups repository.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.ProductionStore.IGroupsRepository" />
    public class GroupsRepository : IGroupsRepository
    {
        private readonly DbContextOptionsBuilder<WppsqldbContext> optionsBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupsRepository"/> class.
        /// </summary>
        public GroupsRepository()
        {
            this.optionsBuilder = new DbContextOptionsBuilder<WppsqldbContext>();
        }

        /// <inheritdoc />
        public async Task AddGroup(Groups group)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                await db.Groups.AddAsync(group);
                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public async Task AddGroupRange(List<Groups> groupList)
        {
            using (var db = new WppsqldbContext(this.optionsBuilder.Options))
            {
                await db.Groups.AddRangeAsync(groupList);
                await db.SaveChangesAsync();
            }
        }
    }
}
