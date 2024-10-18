using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class PetroleumBusinessStore
    {
        public Guid PetroleumDetailId { get; set; }
        public Guid PetroleumBusinessId { get; set; }
        public string TenCuaHang { get; set; } = null!;
        public string? NguoiDaiDien { get; set; }
        public string? SoDienThoai { get; set; }
        public Guid? Huyen { get; set; }
        public Guid? Xa { get; set; }
        public string? NguoiLienHeDonViCungCap { get; set; }
        public string? DiaChiDonViCungCap { get; set; }
        public string? DonViCungCap { get; set; }
        public string? SoDienThoaiDonViCungCap { get; set; }
        public Guid? HinhThucHopDong { get; set; }
        public string? DiaChi { get; set; }
        public string? GiayPhepKinhDoanh { get; set; }
        public DateTime? NgayCapPhep { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public Guid? LoaiGiayXacNhan { get; set; }
        public DateTime? ThoiHan1Nam { get; set; }
        public DateTime? ThoiHan5Nam { get; set; }
        public string? NguoiQuanLy { get; set; }
        public Guid? HinhThuc { get; set; }
        public int? SoCotBomE5 { get; set; }
        public int? SoCotBomA95 { get; set; }
        public int? SoCotBomOil { get; set; }
        public int? SoBeChua { get; set; }
        public int? TongDungTich { get; set; }
        public string? ThoiGianBanHang { get; set; }
        public string? DienTichXayDung { get; set; }
        public string? TuyenPhucVu { get; set; }
        public string? GhiChu { get; set; }
        public DateTime? NgayCapPhepXayDung { get; set; }
        /// <summary>
        /// 1: Đã xóa; 0: Chưa xóa
        /// </summary>
        public bool IsDel { get; set; }
        public Guid? CreateUserId { get; set; }
    }
}
