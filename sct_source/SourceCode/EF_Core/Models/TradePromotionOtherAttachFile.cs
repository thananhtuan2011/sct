using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradePromotionOtherAttachFile
    {
        public Guid TradePromotionOtherAttachFileId { get; set; }
        public Guid TradePromotionOtherId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
