using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradePromotionProgramForBusinessDetail
    {
        public Guid TradePromotionProgramForBusinessDetailId { get; set; }
        public Guid TradePromotionProgramBusinessId { get; set; }
        public Guid Profession { get; set; }
    }
}
