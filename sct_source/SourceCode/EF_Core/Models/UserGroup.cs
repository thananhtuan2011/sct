using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class UserGroup
    {
        public Guid GroupId { get; set; }
        public string UserName { get; set; } = null!;
    }
}
