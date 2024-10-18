using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class SysColumn
    {
        public string TableKey { get; set; } = null!;
        public string ColumnKey { get; set; } = null!;
        public string? ColumnName { get; set; }
        public bool? IsNull { get; set; }
        public bool? IsExist { get; set; }
        public string? RefId { get; set; }
        public string? RefTable { get; set; }
        public string? RefName { get; set; }
        public string? PrimaryRefId { get; set; }
        public string? DataType { get; set; }
    }
}
