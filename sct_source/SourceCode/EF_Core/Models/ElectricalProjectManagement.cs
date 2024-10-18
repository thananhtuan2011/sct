using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ElectricalProjectManagement
    {
        /// <summary>
        /// Bảng quản lý công trình điện 110 KV và 220 KV trên tỉnh
        /// </summary>
        public Guid ElectricalProjectManagementId { get; set; }
        /// <summary>
        /// Mã công trình
        /// </summary>
        public string? BuildingCode { get; set; }
        /// <summary>
        /// Tên công trình
        /// </summary>
        public string BuildingName { get; set; } = null!;
        public Guid? TypeOfConstruction { get; set; }
        public Guid? VoltageLevel { get; set; }
        public string? Wattage { get; set; }
        public string? Length { get; set; }
        public string? WireType { get; set; }
        /// <summary>
        /// Huyện
        /// </summary>
        public Guid District { get; set; }
        /// <summary>
        /// Địa điểm
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Người đại diện
        /// </summary>
        public string Represent { get; set; } = null!;
        /// <summary>
        /// Trạng thái: 1: Hoạt động, 2: Tạm ngừng, 3 Ngừng hoạt động
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
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
