// <copyright file="ISddlBuilderUtility.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Wpp.StorageAutomation.Common;
using ProductionStoreEntity = Wpp.StorageAutomation.Entities.Models.ProductionStore;

namespace Wpp.StorageAutomation.Security
{
    /// <summary>
    /// Security interface.
    /// </summary>
    public interface ISddlBuilderUtility
    {
        /// <summary>
        /// Builds the SDDL.
        /// </summary>
        /// <param name="productionStore">The production store.</param>
        /// <param name="activity">The activity.</param>
        /// <returns>SDDL.</returns>
        string BuildSDDL(ProductionStoreEntity productionStore, ActivityType activity);
    }
}
