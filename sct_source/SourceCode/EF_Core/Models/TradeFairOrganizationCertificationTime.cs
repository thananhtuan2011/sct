using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradeFairOrganizationCertificationTime
    {
        /// <summary>
        /// Thời gian tổ chức  - Quản lý xác nhận tổ chức hội chợ triển lãm thương mại - Xúc tiến thương mại
        /// </summary>
        public Guid TradeFairTimeId { get; set; }
        public Guid TradeFairOrganizationCertificationId { get; set; }
        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}
