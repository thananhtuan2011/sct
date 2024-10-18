using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class UuiRefreshToken
    {
        public Guid Uuid { get; set; }
        public string UserName { get; set; } = null!;
    }
}
