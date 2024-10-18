using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ManagementElectricityActivity
    {
        public Guid ManagementElectricityActivitiesId { get; set; }
        /// <summary>
        /// @Mã quốc gia@
        /// </summary>
        public string ProjectName { get; set; } = null!;
        /// <summary>
        /// @Tên quốc gia@
        /// </summary>
        public Guid DistrictId { get; set; }
        public double Wattage { get; set; }
        public double MaxWattage { get; set; }
        public int Type { get; set; }
        public DateTime DateOfAcceptance { get; set; }
        public string ConnectorAgreement { get; set; } = null!;
        public string PowerPurchaseAgreement { get; set; } = null!;
        public string? AnotherContent { get; set; }
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
