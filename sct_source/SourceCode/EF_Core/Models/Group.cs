using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class Group
    {
        public Guid GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
    }
}
