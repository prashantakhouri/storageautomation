// <copyright file="IArchiver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace Wpp.StorageAutomation.ArchiveQueueStorage.Contract
{
    /// <summary>
    /// Archiver.
    /// </summary>
    public interface IArchiver
    {
        /// <summary>
        /// Archives the production asynchronous.
        /// </summary>
        /// <param name="productionId">The production identifier.</param>
        /// <returns>The task.</returns>
        Task ArchiveProductionAsync(string productionId);
    }
}
