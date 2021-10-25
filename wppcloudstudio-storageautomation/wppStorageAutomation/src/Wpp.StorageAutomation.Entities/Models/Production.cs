using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpp.StorageAutomation.Entities.Models
{
    public partial class Production
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProductionStoreId { get; set; }
        public string Wipurl { get; set; }
        public string ArchiveId { get; set; }
        public string ArchiveUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? LastSyncDateTime { get; set; }
        public long? SizeInBytes { get; set; }
        public bool? DeletedFlag { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string GetStatusQueryUri { get; set; }
        public DateTime? StateChangeDateTime { get; set; }

        public virtual ProductionStore ProductionStore { get; set; }
    }
}
