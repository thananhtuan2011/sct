using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class GroupPermit
    {
        public Guid GroupId { get; set; }
        public string Code { get; set; } = null!;
    }
}
