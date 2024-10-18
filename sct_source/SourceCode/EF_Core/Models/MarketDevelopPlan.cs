using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class MarketDevelopPlan
    {
        public Guid MarketDevelopPlanId { get; set; }
        public string MarketName { get; set; } = null!;
        public Guid RankId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
        public string? Address { get; set; }
        public Guid Stage { get; set; }
        public Guid TypeOfPlanMarket { get; set; }
        public decimal? ExistLandArea { get; set; }
        public decimal? NewLandArea { get; set; }
        public decimal? AddLandArea { get; set; }
        public decimal? Capital { get; set; }
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
