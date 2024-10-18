using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradePromotionProjectManagement
    {
        public Guid TradePromotionProjectManagementId { get; set; }
        public string TradePromotionProjectManagementName { get; set; } = null!;
        public string? ImplementingAgencies { get; set; }
        public decimal? Cost { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public int NumberOfApprovalDocuments { get; set; }
        public byte? ImplementationResults { get; set; }
        public byte? Status { get; set; }
        public string? Reason { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
