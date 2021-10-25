// <copyright file="DirectoryStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Wpp.StorageAutomation.Common
{
    /// <summary>
    ///   Item type.
    /// </summary>
    public enum DirectoryStatus
    {
        /// <summary>The folder is on WIP.</summary>
        Online,

        /// <summary>The folder is on Archive.</summary>
        Offline,

        /// <summary>The folder is being made offline.</summary>
        MakingOffline,

        /// <summary>The folder is being made online.</summary>
        MakingOnline,

        /// <summary>The folder is being archived.</summary>
        Archiving,

        /// <summary>The folder has been deleted.</summary>
        Deleted
    }
}
