using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TableResult
    {
        public string? TableName { get; set; }
        public string? FullTableName { get; set; }
        public string? ColumnName { get; set; }
        public string? Comment { get; set; }
        public bool? IsNull { get; set; }
        public string? Ref { get; set; }
        public bool? Exist { get; set; }
    }
}
