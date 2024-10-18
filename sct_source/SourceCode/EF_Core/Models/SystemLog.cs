using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class SystemLog
    {
        public Guid LogId { get; set; }
        public string? ApplicationCode { get; set; }
        public string? ServiceCode { get; set; }
        public string? SessionId { get; set; }
        public string? IpPortParentNode { get; set; }
        public string? IpPortCurrentNode { get; set; }
        public string? RequestConent { get; set; }
        public string? ReponseConent { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Duration { get; set; }
        public int? ErrorCode { get; set; }
        public string? ErrorDescription { get; set; }
        public string? TransactionStatus { get; set; }
        public string? ActionName { get; set; }
        public string? ActionType { get; set; }
        public string? UserName { get; set; }
        public string? Account { get; set; }
        public int? LogLever { get; set; }
        public string? ContentLog { get; set; }
        public bool IsDel { get; set; }
    }
}
