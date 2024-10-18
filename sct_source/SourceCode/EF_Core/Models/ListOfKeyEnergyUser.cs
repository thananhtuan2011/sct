using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ListOfKeyEnergyUser
    {
        /// <summary>
        /// Bảng quản lý cơ sở sử dụng năng lượng trọng điểm
        /// </summary>
        public Guid ListOfKeyEnergyUsersId { get; set; }
        /// <summary>
        /// Id doanh nghiệp
        /// </summary>
        public Guid BusinessId { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Link doanh nghiệp
        /// </summary>
        public string? Link { get; set; }
        public Guid? Profession { get; set; }
        public string? ManufactProfession { get; set; }
        public decimal EnergyConsumption { get; set; }
        public string? Note { get; set; }
        public int? Date { get; set; }
        /// <summary>
        /// Quyết định CSNLTĐ
        /// </summary>
        public string? Decision { get; set; }
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
