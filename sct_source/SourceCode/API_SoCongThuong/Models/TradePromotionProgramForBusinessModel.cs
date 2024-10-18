using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Models
{
    public partial class TradePromotionProgramForBusinessModel
    {
        public Guid TradePromotionProgramBusinessId { get; set; }
        public Guid Business { get; set; }
        public string BusinessName { get; set; } = "";
        public string BusinessProfession { get; set; } = "";
        public Guid Country { get; set; }
        public string CountryName { get; set; } = "";
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
        public List<TradePromotionProgramForBusinessDetailModel> Details { get; set; } = new List<TradePromotionProgramForBusinessDetailModel>();
    }
    public partial class TradePromotionProgramForBusinessDetailModel
    {
        public Guid TradePromotionProgramForBusinessDetailId { get; set; }
        public Guid TradePromotionProgramBusinessId { get; set; }
        public Guid Profession { get; set; }
        public string ProfessionCode { get; set; } = "";
        public string ProfessionName { get; set; } = "";
    }
}
