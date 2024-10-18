using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradePromotionProjectManagementBussiness
    {
        public Guid Id { get; set; }
        public Guid TradePromotionProjectManagementId { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessCode { get; set; } = null!;
        public string BusinessNameVi { get; set; } = null!;
        public string NganhNghe { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public string NguoiDaiDien { get; set; } = null!;
    }
}
