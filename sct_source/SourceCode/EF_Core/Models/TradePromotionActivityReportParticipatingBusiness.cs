using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradePromotionActivityReportParticipatingBusiness
    {
        /// <summary>
        /// Bảng doanh nghiệp tham gia
        /// </summary>
        public Guid ParticipatingBusinessId { get; set; }
        public Guid TradePromotionActivityReportId { get; set; }
        /// <summary>
        /// Id doanh nghiệp, Guid.Empty nếu là doanh nghiệp ngoài tỉnh
        /// </summary>
        public Guid BusinessId { get; set; }
        /// <summary>
        /// Tên doanh nghiệp
        /// </summary>
        public string BusinessName { get; set; } = null!;
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
    }
}
