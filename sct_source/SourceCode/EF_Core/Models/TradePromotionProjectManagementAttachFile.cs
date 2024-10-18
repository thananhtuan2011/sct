using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradePromotionProjectManagementAttachFile
    {
        public Guid TradePromotionProjectManagementAttachFileId { get; set; }
        public Guid TradePromotionProjectManagementId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
