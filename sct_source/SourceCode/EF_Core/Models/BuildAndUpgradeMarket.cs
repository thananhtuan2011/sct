using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class BuildAndUpgradeMarket
    {
        public Guid BuildAndUpgradeId { get; set; }
        public string BuildAndUpgradeName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int? Year { get; set; }
        public Guid? CommercialId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? CommuneId { get; set; }
        public decimal? TotalInvestment { get; set; }
        public string? TotalInvestmentUnit { get; set; }
        public decimal? RealizedCapital { get; set; }
        public string? RealizedCapitalUnit { get; set; }
        public decimal? BudgetCapital { get; set; }
        public string? BudgetCapitalUnit { get; set; }
        public decimal? LandUseCapital { get; set; }
        public string? LandUseCapitalUnit { get; set; }
        public decimal? Loans { get; set; }
        public string? LoansUnit { get; set; }
        public decimal? AnotherCapital { get; set; }
        public string? AnotherCapitalUnit { get; set; }
        /// <summary>
        /// 1: True 0: False
        /// </summary>
        public bool IsBuild { get; set; }
        /// <summary>
        /// 1: True 0: False
        /// </summary>
        public bool IsUpgrade { get; set; }
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
