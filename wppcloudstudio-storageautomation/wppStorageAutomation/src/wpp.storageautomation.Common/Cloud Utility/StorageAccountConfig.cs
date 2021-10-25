// <copyright file="StorageAccountConfig.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Wpp.StorageAutomation.Common
{
    /// <summary>
    /// Storage account config.
    /// </summary>
    /// <seealso cref="Wpp.StorageAutomation.IStorageAccountConfig" />
    public class StorageAccountConfig : IStorageAccountConfig
    {
        private readonly IConfiguration configuration;

        private readonly IHostingEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageAccountConfig"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The environment.</param>
        public StorageAccountConfig(IConfiguration configuration, IHostingEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
            this.GetSQLDBConnectionString();
            this.GetWPPSDDLConfig();
            this.GetIsGraphSIDSFlag();
            this.GetQueueStorageCredentials();
        }

        /// <summary>Gets or sets the archive connection string.</summary>
        /// <value>The app DB connection string.</value>
        public string WPPSQLDBConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the WPPSDDL configuration.
        /// </summary>
        /// <value>
        /// The WPPSDDL configuration.
        /// </value>
        public string WPPSDDLConfig { get; set; }

        /// <summary>
        /// Gets or sets the WPP readonly SDDL configuration.
        /// </summary>
        /// <value>
        /// The WPP readonly SDDL configuration.
        /// </value>
        public string WPPReadonlySDDLConfig { get; set; }

        /// <summary>
        /// Gets or sets the WPP full control SDDL configuration.
        /// </summary>
        /// <value>
        /// The WPP full control SDDL configuration.
        /// </value>
        public string WPPFullControlSDDLConfig { get; set; }

        /// <summary>
        /// Gets or sets the is graph sids.
        /// </summary>
        /// <value>
        /// The is graph sids.
        /// </value>
        public string IsGraphSIDS { get; set; }

        /// <summary>Gets or sets the state duration time.</summary>
        /// <value>The state duration time.</value>
        public string StateDurationTime { get; set; }

        /// <summary>
        /// Gets or sets the QueueStorageConnectionString.
        /// </summary>
        /// <value>
        /// The QueueStorageConnectionString.
        /// </value>
        public string QueueStorageConnectionString { get; set; }

        /// <summary>Gets or sets the QueueStorageName.</summary>
        /// <value>The QueueStorageName.</value>
        public string QueueStorageName { get; set; }

        /// <summary>Gets or sets the Cached StorageConnectionList.</summary>
        /// <value>The StorageConnectionList.</value>
        private static List<KeyVaultSecret> StorageConnectionList { get; set; }

        /// <summary>
        /// Gets the WPPSDDL configuration.
        /// </summary>
        public void GetWPPSDDLConfig()
        {
            this.WPPSDDLConfig = Environment.GetEnvironmentVariable("WPPSDDLConfig");
            this.WPPReadonlySDDLConfig = Environment.GetEnvironmentVariable("WPPReadonlySDDLConfig");
            this.WPPFullControlSDDLConfig = Environment.GetEnvironmentVariable("WPPFullControlSDDLConfig");
        }

        /// <summary>
        /// Gets the is graph sids flag.
        /// </summary>
        public void GetIsGraphSIDSFlag()
        {
            this.IsGraphSIDS = Environment.GetEnvironmentVariable("IsGraphSIDS");
        }

        /// <summary>
        /// Gets the state duration time.
        /// </summary>
        public void GetStateDurationTime()
        {
            this.StateDurationTime = Environment.GetEnvironmentVariable("StateDurationTime");
        }

        /// <summary>
        /// Gets the queue storage credentials.
        /// </summary>
        public void GetQueueStorageCredentials()
        {
            this.QueueStorageConnectionString = Environment.GetEnvironmentVariable("ArchiveQueueStorageAccount");
            this.QueueStorageName = Environment.GetEnvironmentVariable("ArchiveQueueName");
        }

        /// <summary>
        /// GetStorageConnectionString.
        /// </summary>
        /// <param name="storageKeyName">value.</param>
        /// <returns>string.</returns>
        public string GetStorageConnectionString(string storageKeyName)
        {
            var storageConnectionString = this.GetCachedStorageConnection(storageKeyName);
            if (storageConnectionString == null)
            {
#if DEBUG
                var localConnectionString = Environment.GetEnvironmentVariable(storageKeyName);
                if (localConnectionString == null || localConnectionString == string.Empty)
                {
                    throw new KeyNotFoundException($"Key name {storageKeyName} not found in key vault.");
                }

                this.SetCachedStorageConnection(new KeyVaultSecret(storageKeyName, localConnectionString));
                return localConnectionString;
#else
                var task = Task.Run(async () => await this.GetKeyVaultConnectionString(storageKeyName));
                if (task.IsFaulted && task.Exception != null)
                {
                    throw task.Exception;
                }

                storageConnectionString = task.Result;
                if (storageConnectionString == null || storageConnectionString == string.Empty)
                {
                    throw new KeyNotFoundException($"Key name {storageKeyName} not found in key vault.");
                }

                this.SetCachedStorageConnection(new KeyVaultSecret(storageKeyName, storageConnectionString));
#endif
            }

            return storageConnectionString;
        }

        private void GetSQLDBConnectionString()
        {
            this.WPPSQLDBConnectionString = Environment.GetEnvironmentVariable("WPPSQLDBConnection");
        }

        private string GetCachedStorageConnection(string storageKeyName)
        {
            string storageConnString = null;

            if (StorageConnectionList != null && StorageConnectionList.Count > 0)
            {
                var kvseceret = StorageConnectionList.Find(item => item.Name == storageKeyName);
                if (kvseceret != null)
                {
                    storageConnString = kvseceret.Value;
                }
            }

            return storageConnString;
        }

        private void SetCachedStorageConnection(KeyVaultSecret keyVaultSecret)
        {
            if (StorageConnectionList == null)
            {
                StorageConnectionList = new List<KeyVaultSecret>();
            }

            string keyValue = Convert.ToString(keyVaultSecret.Value);
            if (keyValue != string.Empty && (StorageConnectionList.Find(x => x.Name == keyVaultSecret.Name) == null))
            {
                StorageConnectionList.Add(keyVaultSecret);
            }
        }

        // DO NOT Delete this method as it shows blur in DEBUG MODE
        private async Task<string> GetKeyVaultConnectionString(string storageKeyName)
        {
            string keyVaultUri = Environment.GetEnvironmentVariable("WPPStorageKeyVaultUri");

            // DONT DELETE for now (app reg method )
            // string clientId  = Environment.GetEnvironmentVariable("WPPKeyVaultClientId");.
            // string clientSecret = Environment.GetEnvironmentVariable("WPPKeyVaultClientSecret");.
            // string tenantId = Environment.GetEnvironmentVariable("WPPKeyVaultTenantId");.
            // var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);.
            var credential = new DefaultAzureCredential();

            var secretClient = new SecretClient(new Uri(keyVaultUri), credential);
            var secretManager = new SecretManager(secretClient);
            string secretValue = await secretManager.GetSecretAsync(storageKeyName);

            return secretValue;
        }
    }
}
