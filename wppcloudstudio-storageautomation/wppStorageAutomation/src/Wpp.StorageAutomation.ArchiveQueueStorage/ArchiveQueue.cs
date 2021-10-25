// <copyright file="ArchiveQueue.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Azure.Storage.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wpp.StorageAutomation.ArchiveQueueStorage.Contract;
using Wpp.StorageAutomation.ArchiveQueueStorage.Models;
using Wpp.StorageAutomation.Common;

namespace Wpp.StorageAutomation.ArchiveQueueStorage
{
    /// <summary>
    /// ArchiveQueue.
    /// </summary>
    public class ArchiveQueue
    {
        private readonly IArchiver archiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveQueue"/> class.
        /// </summary>
        /// <param name="archiver">The archiver.</param>
        public ArchiveQueue(IArchiver archiver)
        {
            this.archiver = archiver;
        }

        /// <summary>
        /// Runs the specified my queue item.
        /// </summary>
        /// <param name="queueItem">My queue item.</param>
        /// <param name="log">The log.</param>
        /// <param name="id">The message id.</param>
        /// <returns>The Task.</returns>
        [FunctionName("ArchiveQueue")]

        [StorageAccount("ArchiveQueueStorageAccount")]
        public async Task Run([QueueTrigger("%ArchiveQueueName%")] string queueItem, ILogger log, string id)
        {
            if (!string.IsNullOrEmpty(queueItem))
            {
                log.LogInformation($"ArchiveQueue trigger function processing for id: {id} and message: {queueItem}");
                var startTime = DateTime.UtcNow;
                try
                {
                    ArchiveMessage archiveMessage = JsonConvert.DeserializeObject<ArchiveMessage>(queueItem);
                    log.LogInformation($"ArchiveProductionAsync called for {archiveMessage.ProductionName}");
                    await this.archiver.ArchiveProductionAsync(archiveMessage.ProductionId);
                }
                catch (Exception ex)
                {
                    log.LogError($"Error  while archiving a production : {ex.Message}");
                }

                log.LogInformation($"ArchiveQueue trigger function processed: {queueItem}. Took : {DateTime.UtcNow - startTime} mins");
            }
        }
    }
}
