using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class ParticipateSupportFairModel
    {
        public Guid ParticipateSupportFairId { get; set; }
        public string ParticipateSupportFairName { get; set; } = "";
        public string Address { get; set; } = null!;
        public Guid Country { get; set; }
        public string CountryName { get; set; } = "";
        public string Scale { get; set; } = "";
        public DateTime StartTime { get; set; }
        public string StartTimeDisplay { get; set; } = "";
        public DateTime? EndTime { get; set; }
        public string EndTimeDisplay { get; set; } = "";
        /// <summary>
        /// 1: Sở tham gia
        /// 2: Hỗ trợ doanh nghiệp tham gia
        /// </summary>
        public int PlanJoin { get; set; }
        public string PlanJoinName { get; set; } = "";
        public int NumberOfBusiness { get; set; }
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
        public List<ParticipateSupportFairDetailModel> Details { get; set; } = new List<ParticipateSupportFairDetailModel>();
        public Guid? DistrictId { get; set; }
        public Guid? CommuneId { get; set; }
        public long ImplementCost { get; set; }
        public int Type { get; set; } = 0; // 1: Chỉ gồm Doanh trong tỉnh, 2: chỉ gồm Doanh nghiệp ngoài tỉnh , 0: Cả trong và ngoài tỉnh
    }
    public partial class ParticipateSupportFairDetailModel
    {
        public Guid ParticipateSupportFairDetailId { get; set; }
        public Guid ParticipateSupportFairId { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessCode { get; set; } = null!;
        public string BusinessNameVi { get; set; } = null!;
        public string NganhNghe { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public string NguoiDaiDien { get; set; } = null!;
        public string Huyen { get; set; } = "";
        public string Xa { get; set; } = "";
    }
}
