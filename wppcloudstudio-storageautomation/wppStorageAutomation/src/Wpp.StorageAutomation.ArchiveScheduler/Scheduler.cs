// <copyright file="Scheduler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wpp.StorageAutomation.Common;
using Wpp.StorageAutomation.Entities.Models;

namespace Wpp.StorageAutomation.ArchiveScheduler
{
    /// <summary>
    /// The scheduler for archive.
    /// </summary>
    public class Scheduler
    {
        private readonly IProductionRepository productionRepository;
        private readonly IStorageAccountConfig storageAccountConfig;
        private readonly ICloudStorageUtility cloudStorageUtility;
        private QueueClient queueClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="productionRepository">The production repository.</param>
        /// <param name="storageAccountConfig">The storage account configuration.</param>
        /// <param name="cloudStorageUtility">The cloud storage utility.</param>
        public Scheduler(IProductionRepository productionRepository, IStorageAccountConfig storageAccountConfig, ICloudStorageUtility cloudStorageUtility)
        {
            this.productionRepository = productionRepository;
            this.storageAccountConfig = storageAccountConfig;
            this.cloudStorageUtility = cloudStorageUtility;
        }

        /// <summary>
        /// Runs the specified my timer.
        /// </summary>
        /// <param name="myTimer">My timer.</param>
        /// <param name="log">The log.</param>
        /// <returns>A task.</returns>
        [FunctionName("ArchiveScheduler")]
        public async Task Run([TimerTrigger("%SchedularTimerInterval%", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            var startTime = DateTime.UtcNow;
            try
            {
                log.LogInformation($"Scheduled archive initiated: {DateTime.UtcNow}");

                string stateDurationTime = Environment.GetEnvironmentVariable("StateDurationTime");
                double interval = Convert.ToDouble(stateDurationTime);

                log.LogInformation($"Scheduler: SQL fetch for valid productions starting.");
                var onlineProds = await this.productionRepository.GetAllValidProductions(interval);
                log.LogInformation($"Scheduler: SQL fetch for valid productions completed.");

                this.queueClient = this.cloudStorageUtility.CreateQueueClient(this.storageAccountConfig.QueueStorageConnectionString, this.storageAccountConfig.QueueStorageName);
                await this.queueClient.CreateIfNotExistsAsync();

                log.LogInformation($"Connected to queue client for archival.");

                foreach (var production in onlineProds)
                {
                    await this.SendMessageToQueue(production, log);
                }

                log.LogInformation($"All messages for valid productions sent to queue for archival. Took: {DateTime.UtcNow - startTime}");
            }
            catch (Exception ex)
            {
                log.LogError(default, ex, ex.Message);
                log.LogInformation($"An exception occured at scheduler level after: {DateTime.UtcNow - startTime} ");
            }
        }

        private async Task SendMessageToQueue(Production production, ILogger log)
        {
            log.LogInformation($"Sending message to queue started for: {production.Name} at: {DateTime.UtcNow}");
            ArchiveMessage archiveMessage = new ArchiveMessage
            {
                ProductionId = production.Id,
                ProductionStoreId = production.ProductionStoreId,
                ProductionName = production.Name,
            };

            string productionMessage = JsonConvert.SerializeObject(archiveMessage);
            string encodedMsg = Convert.ToBase64String(Encoding.UTF8.GetBytes(productionMessage));
            log.LogInformation($"Encoded: {encodedMsg} for original message: {productionMessage}");
            if (await this.queueClient.ExistsAsync())
            {
                Response<SendReceipt> response = await this.queueClient.SendMessageAsync(encodedMsg);
                log.LogInformation($"Message inserted successfully for :{production.Name}  at {response.Value.InsertionTime}");
            }
        }
    }
}
