using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradePromotionActivityReportAttachFile
    {
        public Guid TradePromotionActivityReportAttachFileId { get; set; }
        public Guid TradePromotionActivityReportId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
