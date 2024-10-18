using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class Business
    {
        public Guid BusinessId { get; set; }
        public string BusinessCode { get; set; } = null!;
        public string TenGiaoDich { get; set; } = null!;
        public string? BusinessNameEn { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? CommuneId { get; set; }
        public string? GiayPhepSanXuat { get; set; }
        public string? DiaChiTruSo { get; set; }
        public DateTime? NgayCapPhep { get; set; }
        public string? MaSoThue { get; set; }
        public string BusinessNameVi { get; set; } = null!;
        public Guid LoaiHinhDoanhNghiep { get; set; }
        public Guid? LoaiNganhNghe { get; set; }
        public DateTime? NgayHoatDong { get; set; }
        public string? NguoiDaiDien { get; set; }
        public string? SoDienThoai { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? Cccd { get; set; }
        public DateTime? NgayCap { get; set; }
        public string? NoiCap { get; set; }
        public string? DiaChi { get; set; }
        public string? GiamDoc { get; set; }
        public string? Email { get; set; }
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
