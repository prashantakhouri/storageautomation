// <copyright file="IStorageAccountConfig.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Wpp.StorageAutomation.Common
{
    /// <summary>
    /// Storage account config.
    /// </summary>
    public interface IStorageAccountConfig
    {
        /// <summary>
        /// Gets or sets the WPPSDDL configuration.
        /// </summary>
        /// <value>
        /// The WPPSDDL configuration.
        /// </value>
        string WPPSDDLConfig { get; set; }

        /// <summary>
        /// Gets or sets the WPP readonly SDDL configuration.
        /// </summary>
        /// <value>
        /// The WPP readonly SDDL configuration.
        /// </value>
        string WPPReadonlySDDLConfig { get; set; }

        /// <summary>
        /// Gets or sets the WPP readonly SDDL configuration.
        /// </summary>
        /// <value>
        /// The WPP full control SDDL configuration.
        /// </value>
        string WPPFullControlSDDLConfig { get; set; }

        /// <summary>
        /// Gets or sets the is graph sids.
        /// </summary>
        /// <value>
        /// The is graph sids.
        /// </value>
        string IsGraphSIDS { get; set; }

        /// <summary>Gets or sets the state duration time.</summary>
        /// <value>The state duration time.</value>
        public string StateDurationTime { get; set; }

        /// <summary>
        /// Gets or sets the QueueStorageConnectionString.
        /// </summary>
        /// <value>
        /// The is QueueStorageConnectionString.
        /// </value>
        string QueueStorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the QueueStorageName.
        /// </summary>
        /// <value>
        /// The is QueueStorageName.
        /// </value>
        string QueueStorageName { get; set; }

        /// <summary>
        /// GetStorageConnectionString.
        /// </summary>
        /// <param name="storageKeyName"> The storage key name. </param>
        /// <returns>StorageConnectionString.</returns>
        string GetStorageConnectionString(string storageKeyName);
    }
}
