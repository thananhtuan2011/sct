using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class AlcoholBussinessDetail
    {
        public Guid AlcoholBusinessDetailId { get; set; }
        public Guid AlcoholBusinessId { get; set; }
        public string TenDoanhNghiep { get; set; } = null!;
        public string NguoiDaiDien { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public Guid Huyen { get; set; }
        public Guid Xa { get; set; }
        public string? DiaChi { get; set; }
        public string? GiayPhepKinhDoanh { get; set; }
        public DateTime? NgayCapGiayPhepBanLe { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string? DonViCungCap { get; set; }
        public string? DiaChiDonViCungCap { get; set; }
        public string? SoDienThoaiDonViCungCap { get; set; }
        public string? GhiChu { get; set; }
        /// <summary>
        /// 1: Đã xóa; 0: Chưa xóa
        /// </summary>
        public bool IsDel { get; set; }
        public Guid? CreateUserId { get; set; }
    }
}
