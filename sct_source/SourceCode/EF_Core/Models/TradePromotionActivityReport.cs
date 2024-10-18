using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradePromotionActivityReport
    {
        /// <summary>
        /// Bảng báo cáo hoạt động xúc tiến thương mại
        /// </summary>
        public Guid TradePromotionActivityReportId { get; set; }
        /// <summary>
        /// Id quy mô: 0 - Trong tỉnh, 1 - Ngoài tỉnh, 2 - Ngoài nước
        /// </summary>
        public int ScaleId { get; set; }
        /// <summary>
        /// Tên đề án
        /// </summary>
        public string PlanName { get; set; } = null!;
        /// <summary>
        /// Kế hoạch tham gia
        /// </summary>
        public bool PlanToJoin { get; set; }
        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Thời gian kết thúc
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Thời lượng
        /// </summary>
        public string? Time { get; set; }
        /// <summary>
        /// Id huyện
        /// </summary>
        public Guid DistrictId { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Kinh phí thực hiện
        /// </summary>
        public long ImplementationCost { get; set; }
        /// <summary>
        /// Kinh phí hổ trợ
        /// </summary>
        public long FundingSupport { get; set; }
        /// <summary>
        /// Quy mô
        /// </summary>
        public string? Scale { get; set; }
        /// <summary>
        /// Số lượng doanh nghiệp tham gia
        /// </summary>
        public int NumParticipating { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// 1: Hoạt động 0: Không hoạt động
        /// </summary>
        public bool? IsAction { get; set; }
        /// <summary>
        /// 1: Đã xóa; 0: Chưa xóa
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
