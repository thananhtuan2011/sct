using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class IndustrialPromotionProjectModel
    {
        public Guid IndustrialPromotionProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        /// <summary>
        /// Thời gian bắt đầu
        /// </summary>
        public string StartDate { get; set; } = null!;
        public DateTime? StartDateTime { get; set; }
        /// <summary>
        /// Thời gian kêt
        /// </summary>
        public string EndDate { get; set; } = null!;
        public DateTime? EndDateTime { get; set; }
        /// <summary>
        /// Năm bắt đầu
        /// </summary>
        public int? StartYear { get; set; }
        /// <summary>
        /// 1: Trung ương
        /// 2: Địa phương
        /// </summary>
        public int Capital { get; set; }
        public string CapitalName { get; set; } = "";
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
        public string CreateTimeDisplay { get; set; } = "";
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public string CreateName { get; set; } = "";
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<IndustrialPromotionProjectDetailModel> Details { get; set; } = new List<IndustrialPromotionProjectDetailModel>();
    }
    public partial class IndustrialPromotionProjectDetailModel
    {
        public Guid IndustrialPromotionProjectDetailId { get; set; }
        public Guid IndustrialPromotionProjectId { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessCode { get; set; } = null!;
        public string BusinessNameVi { get; set; } = null!;
        public string NganhNghe { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public string NguoiDaiDien { get; set; } = null!;
    }
}
