using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateProjectHistory
    {
        public Guid CateProjectHistoryId { get; set; }
        public Guid CateProjectId { get; set; }
        public string ContentAdjust { get; set; } = null!;
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
