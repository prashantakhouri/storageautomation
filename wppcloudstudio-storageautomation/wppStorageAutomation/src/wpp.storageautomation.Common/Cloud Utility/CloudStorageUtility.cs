// <copyright file="CloudStorageUtility.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.Azure.Storage.File;

namespace Wpp.StorageAutomation.Common
{
    /// <summary>
    ///   The cloud storage utility.
    /// </summary>
    public class CloudStorageUtility : ICloudStorageUtility
    {
        /// <summary>
        /// Creates the queue client.
        /// </summary>
        /// <param name="queueConnectionString">The queueConnectionString.</param>
        /// <param name="queueName">The queueName.</param>
        /// <returns>
        /// The cloud queue client.
        /// </returns>
        public QueueClient CreateQueueClient(string queueConnectionString, string queueName)
        {
            QueueClient queueClient = new QueueClient(queueConnectionString, queueName);
            return queueClient;
        }

        /// <summary>
        /// Gets the file client.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns>
        /// The cloud file client.
        /// </returns>
        public CloudFileClient GetFileClient(CloudStorageAccount account)
        {
            var fileClient = account.CreateCloudFileClient();
            return fileClient;
        }

        /// <inheritdoc/>
        public CloudBlobDirectory GetBlobContainer(CloudStorageAccount account, string containerName)
        {
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var rootBlobDirectory = container.GetDirectoryReference(string.Empty);
            return rootBlobDirectory;
        }

        /// <inheritdoc/>
        public CloudFileDirectory GetCloudFileShare(CloudStorageAccount account, string fileShareName)
        {
            var fileClient = account.CreateCloudFileClient();
            var fileShare = fileClient.GetShareReference(fileShareName);
            var rootShareDir = fileShare.GetRootDirectoryReference();
            return rootShareDir;
        }

        /// <inheritdoc/>
        public CloudBlobDirectory GetBlobDirectory(CloudStorageAccount account, string containerName, string directoryName)
        {
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var rootBlobDirectory = container.GetDirectoryReference(string.Empty);
            var productionBlobDirectory = rootBlobDirectory.GetDirectoryReference(directoryName);
            return productionBlobDirectory;
        }

        /// <inheritdoc/>
        public CloudFileDirectory GetCloudFileDirectory(CloudStorageAccount account, string fileShareName, string directoryName)
        {
            var fileClient = account.CreateCloudFileClient();
            var fileShare = fileClient.GetShareReference(fileShareName);
            var rootShareDir = fileShare.GetRootDirectoryReference();
            var rootProductionDir = rootShareDir.GetDirectoryReference(directoryName);
            return rootProductionDir;
        }

        /// <inheritdoc/>
        public IEnumerable<CloudFileShare> GetCloudFileShareList(CloudStorageAccount account)
        {
            CloudFileClient fileClient = account.CreateCloudFileClient();
            IEnumerable<CloudFileShare> fileshares = fileClient.ListShares();
            return fileshares;
        }

        /// <inheritdoc/>
        public IEnumerable<CloudBlobContainer> GetCloudBlobContainerList(CloudStorageAccount account)
        {
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            IEnumerable<CloudBlobContainer> blobContainers = blobClient.ListContainers();
            return blobContainers;
        }

        /// <inheritdoc/>
        public async Task DeleteFileShareDirectory(CloudFileDirectory directory)
        {
            if (directory.Exists())
            {
                foreach (IListFileItem item in directory.ListFilesAndDirectories())
                {
                    switch (item)
                    {
                        case CloudFile file:
                            await file.DeleteAsync();
                            break;
                        case CloudFileDirectory dir:
                            await this.DeleteFileShareDirectory(dir);
                            break;
                    }
                }

                await directory.DeleteAsync();
            }
        }

        /// <inheritdoc/>
        public DirectoryTransferContext GetDirectoryTransferContext(TransferCheckpoint checkpoint)
        {
            var context = new DirectoryTransferContext(checkpoint);

            context.ProgressHandler = new Progress<TransferStatus>((progress) =>
            {
            });

            return context;
        }

        /// <inheritdoc/>
        public async Task<bool> BlobDirectoryExists(CloudStorageAccount account, string containerName, string directoryName)
        {
            bool flag = false;
            var blobClient = account.CreateCloudBlobClient();
            bool containerExists = await blobClient.GetContainerReference(containerName).ExistsAsync();
            if (containerExists)
            {
                var container = blobClient.GetContainerReference(containerName);
                var rootBlobDirectory = container.GetDirectoryReference(string.Empty);
                var productionBlobDirectory = rootBlobDirectory.GetDirectoryReference(directoryName);
                var listCount = productionBlobDirectory.ListBlobs().Any();
                if (listCount)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }

            return flag;
        }

        /// <inheritdoc/>
        public async Task<bool> FileShareDirectoryExists(CloudStorageAccount account, string fileShareName, string fileDirectoryName)
        {
            var fileClient = account.CreateCloudFileClient();
            var fileShare = fileClient.GetShareReference(fileShareName);
            var rootShareDir = fileShare.GetRootDirectoryReference();
            var rootProductionDir = rootShareDir.GetDirectoryReference(fileDirectoryName);
            bool fileDirectoryExists = await rootProductionDir.ExistsAsync();
            return fileDirectoryExists;
        }

        /// <inheritdoc/>
        public async Task<bool> FileShareExists(CloudStorageAccount account, string fileShareName)
        {
            var fileClient = account.CreateCloudFileClient();
            bool fileShareExists = await fileClient.GetShareReference(fileShareName).ExistsAsync();
            return fileShareExists;
        }

        /// <inheritdoc/>
        public long GetFileShareByteCount(CloudFileDirectory directory)
        {
            long bytesCount = 0;
            if (directory.Exists())
            {
                foreach (IListFileItem item in directory.ListFilesAndDirectories())
                {
                    switch (item)
                    {
                        case CloudFile file:
                            bytesCount += file.Properties.Length;
                            break;
                        case CloudFileDirectory dir:
                            bytesCount += this.GetFileShareByteCount(dir);
                            break;
                    }
                }
            }

            return bytesCount;
        }
    }
}
