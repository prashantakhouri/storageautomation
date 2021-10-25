using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpp.StorageAutomation.Entities.Models
{
    public partial class ProductionStore
    {
        public ProductionStore()
        {
            Production = new HashSet<Production>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string Wipurl { get; set; }
        public decimal? WipallocatedSize { get; set; }
        public string ArchiveUrl { get; set; }
        public decimal? ArchiveAllocatedSize { get; set; }
        public DateTime? ScaleDownTime { get; set; }
        public DateTime? ScaleUpTimeInterval { get; set; }
        public decimal? MinimumFreeSize { get; set; }
        public decimal? MinimumFreeSpace { get; set; }
        public string OfflineTime { get; set; }
        public string OnlineTime { get; set; }
        public decimal? ProductionOfflineTimeInterval { get; set; }
        public string ManagerRoleGroupNames { get; set; }
        public string UserRoleGroupNames { get; set; }
        public string WipkeyName { get; set; }
        public string ArchiveKeyName { get; set; }

        public virtual ICollection<Production> Production { get; set; }
    }
}
