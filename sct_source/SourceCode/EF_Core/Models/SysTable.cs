using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class SysTable
    {
        public string TableKey { get; set; } = null!;
        public string? TableName { get; set; }
        public string? Url { get; set; }
    }
}
