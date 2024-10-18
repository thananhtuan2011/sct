using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class MarketManagementDetail
    {
        public Guid MarketManagementDetailId { get; set; }
        public Guid MarketId { get; set; }
        public Guid BusinessLineId { get; set; }
        public string BusinessLineName { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsDel { get; set; }
    }
}
