using System;
using System.Collections.Generic;
using System.Text;

namespace Wpp.StorageAutomation
{
    /// <summary>
    /// Gets  the production list response.
    /// </summary>
    public class ProductionListResponse
    {
        /// <summary>
        /// Gets or sets the production list.
        /// </summary>
        /// <value>
        /// The production list.
        /// </value>
        public IEnumerable<ProductionRow> ProductionList { get; set; }
    }

    /// <summary>
    ///   <para>The production row in SQL DB.</para>
    ///   <para>
    ///     <br />
    ///   </para>
    /// </summary>
    public class ProductionRow
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the ProductionStore Id.</summary>
        /// <value>The ProductionStoreId.</value>
        public string ProductionStoreId { get; set; }

        /// <summary>Gets or sets the Wipurl.</summary>
        /// <value>The Wipurl.</value>
        public string Wipurl { get; set; }

        /// <summary>Gets or sets the ArchiveId.</summary>
        /// <value>The ArchiveId.</value>
        public string ArchiveId { get; set; }

        /// <summary>Gets or sets the ArchiveUrl.</summary>
        /// <value>The ArchiveUrl.</value>
        public string ArchiveUrl { get; set; }

        /// <summary>Gets or sets the Status.</summary>
        /// <value>The Status.</value>
        public string Status { get; set; }

        /// <summary>Gets or sets the CreatedDateTime.</summary>
        /// <value>The CreatedDateTime.</value>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>Gets or sets the LastSyncDateTime.</summary>
        /// <value>The LastSyncDateTime.</value>
        public DateTime? LastSyncDateTime { get; set; }

        /// <summary>Gets or sets the Size.</summary>
        /// <value>The Size.</value>
        public long? SizeInBytes { get; set; }
    }
}
