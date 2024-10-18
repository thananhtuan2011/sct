using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class InternationalCommerce
    {
        public Guid InternationalCommerceId { get; set; }
        public Guid InternationalCommerceName { get; set; }
        public string InvestorName { get; set; } = null!;
        public string? Address { get; set; }
        public string? LicensingActivity { get; set; }
        public string? TenCoSoBanLe { get; set; }
        public string? DiaChiCoSoBanLe { get; set; }
        public Guid? LoaiHinhCoSo { get; set; }
        public string? GiayPhepKinhDoanh { get; set; }
        public DateTime? NgayCapGiayPhepKinhDoanh { get; set; }
        public string? GiayPhepBanLe { get; set; }
        public DateTime? NgayCapGiayPhepBanLe { get; set; }
        public DateTime? NgayHetHanGiayPhepBanLe { get; set; }
        public decimal DienTichSuDung { get; set; }
        public decimal DienTichSan { get; set; }
        public decimal DienTichBanHang { get; set; }
        public decimal DienTichKinhDoanh { get; set; }
        public string? GhiChu { get; set; }
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
