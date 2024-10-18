using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ExportGood
    {
        public Guid ExportGoodsId { get; set; }
        public string ExportGoodsName { get; set; } = null!;
        /// <summary>
        /// Get data from Table Category
        /// </summary>
        public Guid ItemGroupId { get; set; }
        /// <summary>
        /// Get data from Table Category
        /// </summary>
        public Guid TypeOfEconomicId { get; set; }
        /// <summary>
        /// Get data from Table Business
        /// </summary>
        public Guid BusinessId { get; set; }
        /// <summary>
        /// Get data from Table Country
        /// </summary>
        public Guid CountryId { get; set; }
        public decimal Amount { get; set; }
        public Guid AmountUnit { get; set; }
        public decimal Price { get; set; }
        public DateTime ExportTime { get; set; }
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
