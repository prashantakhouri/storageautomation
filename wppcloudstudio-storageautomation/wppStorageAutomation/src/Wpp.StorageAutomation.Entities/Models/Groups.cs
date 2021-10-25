using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wpp.StorageAutomation.Entities.Models
{
    public partial class Groups
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string GroupSid { get; set; }
    }
}
