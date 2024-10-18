using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class IndustrialPromotionProject
    {
        public Guid IndustrialPromotionProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 1: Trung ương
        /// 2: Địa phương
        /// </summary>
        public int Capital { get; set; }
        /// <summary>
        /// Tổng kinh phí
        /// </summary>
        public long Funding { get; set; }
        /// <summary>
        /// Kinh phí khuyến công hỗ trợ
        /// </summary>
        public long IndustrialPromotionFunding { get; set; }
        /// <summary>
        /// Kinh phí doanh nghiệp đối ứng
        /// </summary>
        public long ReciprocalEnterpriseFunding { get; set; }
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
