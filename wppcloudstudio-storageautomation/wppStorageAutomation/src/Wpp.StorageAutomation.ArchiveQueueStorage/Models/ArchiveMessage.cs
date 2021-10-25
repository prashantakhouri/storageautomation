// <copyright file="ArchiveMessage.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Wpp.StorageAutomation.Common;

namespace Wpp.StorageAutomation.ArchiveQueueStorage.Models
{
    /// <summary>
    ///   The archive message.
    /// </summary>
    public class ArchiveMessage
    {
        /// <summary>Gets or sets the production store id.</summary>
        /// <value>The production store id.</value>
        public string ProductionStoreId { get; set; }

        /// <summary>Gets or sets the production id.</summary>
        /// <value>The production id.</value>
        public string ProductionId { get; set; }

        /// <summary>Gets or sets the production name.</summary>
        /// <value>The production name.</value>
        public string ProductionName { get; set; }
    }
}
