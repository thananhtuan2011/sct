using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class Target7
    {
        /// <summary>
        /// Bảng tiêu chí số 7
        /// </summary>
        public Guid Target7Id { get; set; }
        /// <summary>
        /// Năm báo cáo
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// ID giai đoạn lấy từ bảng Config
        /// </summary>
        public Guid? StageId { get; set; }
        /// <summary>
        /// ID Huyện lấy từ bảng District
        /// </summary>
        public Guid DistrictId { get; set; }
        /// <summary>
        /// Mã xã lấy từ bảng Commune
        /// </summary>
        public Guid CommuneId { get; set; }
        /// <summary>
        /// Số chợ trong quy hoạch
        /// </summary>
        public int MarketInPlaning { get; set; }
        /// <summary>
        /// Đạt cơ sở hạ tầng thương mại nông thôn mới - Theo kế hoạch
        /// </summary>
        public bool PlanCommercial { get; set; }
        /// <summary>
        /// Chợ đạt chuẩn nông thôn mới - Kế hoạch
        /// </summary>
        public bool PlanMarket { get; set; }
        /// <summary>
        /// Đạt cơ sở hạ tầng thương mại nông thôn mới - Ước tính
        /// </summary>
        public bool EstimateCommercial { get; set; }
        /// <summary>
        /// Chợ đạt chuẩn nông thôn mới - Ước tính
        /// </summary>
        public bool EstimateMarket { get; set; }
        public bool NewRuralCriteriaRaised { get; set; }
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
