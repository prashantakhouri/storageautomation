// <copyright file="PropertiesInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;

namespace Wpp.StorageAutomation.DataMovement.Models
{
    /// <summary>
    /// Blob properties DTO.
    /// </summary>
    public class PropertiesInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the last write time.
        /// </summary>
        /// <value>
        /// The last write time.
        /// </value>
        public DateTimeOffset? LastWriteTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>
        /// The creation time.
        /// </value>
        public DateTimeOffset? CreationTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the absolute path.
        /// </summary>
        /// <value>
        /// The absolute path.
        /// </value>
        public string AbsolutePath { get; set; }
    }
}
