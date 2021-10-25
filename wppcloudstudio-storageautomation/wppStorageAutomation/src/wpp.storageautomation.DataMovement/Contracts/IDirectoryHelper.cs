// <copyright file="IDirectoryHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.File;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Entities.Models;

namespace Wpp.StorageAutomation.DataMovement.Contracts
{
    /// <summary>
    ///   The directory helper.
    /// </summary>
    public interface IDirectoryHelper
    {
        /// <summary>
        /// Transfers files between Azure Files and Blobs based on data movement type mentioned.
        /// </summary>
        /// <param name="fileDirectory">The file directory.</param>
        /// <param name="blobDirectory">The BLOB directory.</param>
        /// <param name="dataMovementType">Type of the data movement.</param>
        /// <returns>The task.</returns>
        Task TransferFilesAsync(CloudFileDirectory fileDirectory, CloudBlobDirectory blobDirectory, DataMovementType dataMovementType);

        /// <summary>
        /// Preserves the properties archive asynchronous.
        /// </summary>
        /// <param name="fileDirectory">The file directory.</param>
        /// <param name="blobDirectory">The BLOB directory.</param>
        /// <param name="productionId">The production identifier.</param>
        /// <returns>The task.</returns>
        Task PreservePropertiesArchiveAsync(CloudFileDirectory fileDirectory, CloudBlobDirectory blobDirectory, string productionId);

        /// <summary>
        /// Preserves the properties restore asynchronous.
        /// </summary>
        /// <param name="fileDirectory">The file directory.</param>
        /// <param name="blobDirectory">The BLOB directory.</param>
        /// <param name="productionId">The production identifier.</param>
        /// <param name="productionStore">The production store.</param>
        /// <returns>The task.</returns>
        Task PreservePropertiesRestoreAsync(CloudFileDirectory fileDirectory, CloudBlobDirectory blobDirectory, string productionId, ProductionStore productionStore);

        /// <summary>
        /// Deletes from file share directory.
        /// </summary>
        /// <param name="fileDirectory">The file directory.</param>
        /// <returns>
        /// A task.
        /// </returns>
        Task DeleteFromFileShareDirectory(CloudFileDirectory fileDirectory);

        /// <summary>
        /// Sets the production store information asynchronous.
        /// </summary>
        /// <returns>The task.</returns>
        Task SetProductionStoreInfoAsync();
    }
}
