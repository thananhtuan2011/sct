using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class CateIndustrialClusterModel
    {
        public Guid CateIndustrialClustersId { get; set; }
        public string IndustrialClustersName { get; set; } = null!;
        public int OriginalArea { get; set; }
        /// <summary>
        /// Quyết định thành lập
        /// </summary>
        public string EstablishCode { get; set; } = null!;
        /// <summary>
        /// Diện tích mở rộng
        /// </summary>
        public int ExpandedArea { get; set; }
        /// <summary>
        /// Quyết định mở rộng
        /// </summary>
        public string DecisionExpandCode { get; set; } = null!;
        /// <summary>
        /// Diện tích chi tiết
        /// </summary>
        public int DetailedArea { get; set; }
        /// <summary>
        /// quyết định phê duyệt
        /// </summary>
        public string ApprovalDecision { get; set; } = null!;
        /// <summary>
        /// Diện tích đất công nghiệp
        /// </summary>
        public int IndustrialArea { get; set; }
        /// <summary>
        /// Diện tích đã cho thuê
        /// </summary>
        public int RentedArea { get; set; }
        /// <summary>
        /// Tỷ lệ lấp đầy
        /// </summary>
        public int Occupancy { get; set; }
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
        public string CreateTimeDisplay { get; set; } = "";
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public string CreateName { get; set; } = "";
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public Guid? District { get; set; } = Guid.Empty;
    }
}
