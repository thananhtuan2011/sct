using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class Permission
    {
        public string Code { get; set; } = null!;
        public string? PermitName { get; set; }
        public string? Description { get; set; }
        public int? Position { get; set; }
        public string? CodeGroup { get; set; }
        public int? Status { get; set; }
        public bool? Disable { get; set; }
    }
}
