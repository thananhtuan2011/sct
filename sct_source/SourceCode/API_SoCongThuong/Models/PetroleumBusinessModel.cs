namespace API_SoCongThuong.Models
{
    public class PetroleumBusinessModel
    {
        public Guid PetroleumBusinessId { get; set; }
        public Guid PetroleumBusinessName { get; set; }
        public string Supplier { get; set; } = "";
        public string Address { get; set; } = "";
        public string Representative { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public int TotalStore { get; set; }
        public string BusinessNameVi { get; set; } = "";
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<PetroleumBusinessDetailModel> PetroleumBusinessDetail { get; set; } = new List<PetroleumBusinessDetailModel>();
        public string GiayDangKyKinhDoanh { get; set; } = "";
        public DateTime? NgayCap { get; set; } 

    }
    public class PetroleumBusinessDetailModel
    {
        public string TenCuaHang { get; set; } = null!;
        public string? NguoiDaiDien { get; set; }
        public string? SoDienThoai { get; set; }
        public Guid? Huyen { get; set; }
        public Guid? Xa { get; set; }
        public string? TenHuyen { get; set; }
        public string? TenXa { get; set; }
        public string? TenHinhThuc { get; set; }
        public string? DonViCungCap { get; set; }
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
        /// <summary>
        /// 1: Đã xóa; 0: Chưa xóa
        /// </summary>
        public bool IsDel { get; set; }
        public string? NguoiLienHeDonViCungCap { get; set; }
        public string? DiaChiDonViCungCap { get; set; }
        public string? SoDienThoaiDonViCungCap { get; set; }
        public Guid? HinhThucHopDong { get; set; }
        public string? GhiChu { get; set; }
        public DateTime? NgayCapPhepXayDung { get; set; } 
        public string? TenLoaiGiayXacNhan { get; set; }
        public string? TenHinhThucHopDong { get; set; }

    }
    public class removeListPetroleumBusinessItems
    {
        public List<Guid> PetroleumBusinessIds { get; set; }
    }

}
