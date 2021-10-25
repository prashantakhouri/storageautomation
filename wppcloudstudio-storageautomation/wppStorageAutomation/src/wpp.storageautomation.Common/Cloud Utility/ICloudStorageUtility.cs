// <copyright file="ICloudStorageUtility.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.Azure.Storage.File;

namespace Wpp.StorageAutomation.Common
{
    /// <summary>
    ///   Cloud storage utility.
    /// </summary>
    public interface ICloudStorageUtility
    {
        /// <summary>
        /// Creates the queue client.
        /// </summary>
        /// <param name="queueConnectionString">The queueConnectionString.</param>
        /// <param name="queueName">The queueName.</param>
        /// <returns>
        /// The queue client.
        /// </returns>
        QueueClient CreateQueueClient(string queueConnectionString, string queueName);

        /// <summary>
        /// Gets the file client.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns>
        /// The cloud file client.
        /// </returns>
        CloudFileClient GetFileClient(CloudStorageAccount account);

        /// <summary>Gets the BLOB container.</summary>
        /// <param name="account">The account.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>
        ///   The cloud blob directory.
        /// </returns>
        CloudBlobDirectory GetBlobContainer(CloudStorageAccount account, string containerName);

        /// <summary>
        /// Gets the cloud file share.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="fileShareName">Name of the file share.</param>
        /// <returns>The cloud file directory.</returns>
        CloudFileDirectory GetCloudFileShare(CloudStorageAccount account, string fileShareName);

        /// <summary>Gets the BLOB directory.</summary>
        /// <param name="account">The account.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="directoryName">Name of the directory.</param>
        /// <returns>
        ///   The cloud blob directory.
        /// </returns>
        CloudBlobDirectory GetBlobDirectory(CloudStorageAccount account, string containerName, string directoryName);

        /// <summary>Gets the cloud file directory.</summary>
        /// <param name="account">The account.</param>
        /// <param name="fileShareName">Name of the file share.</param>
        /// <param name="directoryName">Name of the directory.</param>
        /// <returns>The cloud file directory.</returns>
        CloudFileDirectory GetCloudFileDirectory(CloudStorageAccount account, string fileShareName, string directoryName);

        /// <summary>Gets the cloud file share list.</summary>
        /// <param name="account">The account.</param>
        /// <returns>
        ///   The cloud file shares.
        /// </returns>
        IEnumerable<CloudFileShare> GetCloudFileShareList(CloudStorageAccount account);

        /// <summary>Gets the cloud BLOB container list.</summary>
        /// <param name="account">The account.</param>
        /// <returns>
        ///   The cloud blob container.
        /// </returns>
        IEnumerable<CloudBlobContainer> GetCloudBlobContainerList(CloudStorageAccount account);

        /// <summary>
        /// Deletes the file share directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>The task.</returns>
        Task DeleteFileShareDirectory(CloudFileDirectory directory);

        /// <summary>Gets the directory transfer context.</summary>
        /// <param name="checkpoint">The checkpoint.</param>
        /// <returns>
        ///   The directory transfer context.
        /// </returns>
        DirectoryTransferContext GetDirectoryTransferContext(TransferCheckpoint checkpoint);

        /// <summary>
        /// BLOBs the directory exists.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="directoryName">Name of the directory.</param>
        /// <returns>If blob directory exists or not.</returns>
        Task<bool> BlobDirectoryExists(CloudStorageAccount account, string containerName, string directoryName);

        /// <summary>
        /// Files the share directory exists.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="fileShareName">Name of the file share.</param>
        /// <param name="fileDirectoryName">Name of the file directory.</param>
        /// <returns>If fileshare directory exists or not.</returns>
        Task<bool> FileShareDirectoryExists(CloudStorageAccount account, string fileShareName, string fileDirectoryName);

        /// <summary>Files the share exists.</summary>
        /// <param name="account">The account.</param>
        /// <param name="fileShareName">Name of the file share.</param>
        /// <returns>If fileshare exists or not.</returns>
        Task<bool> FileShareExists(CloudStorageAccount account, string fileShareName);

        /// <summary>
        /// Files the share byte count.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>The size of conent inside file directory.</returns>
        long GetFileShareByteCount(CloudFileDirectory directory);
    }
}
