using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateIntegratedManagementHistory
    {
        public Guid CateIntegratedManagementHistoryId { get; set; }
        public Guid CateIntegratedManagementId { get; set; }
        public string ContentAdjust { get; set; } = null!;
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
