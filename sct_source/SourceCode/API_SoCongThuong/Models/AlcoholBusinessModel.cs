namespace API_SoCongThuong.Models
{
    public class AlcoholBusinessModel
    {
        public Guid AlcoholBusinessId { get; set; }
        public Guid AlcoholBusinessName { get; set; }
        public string? Supplier { get; set; } = "";
        public string? Representative { get; set; } = "";
        public string? PhoneNumber { get; set; } = "";
        public string? Address { get; set; } = "";
        public string BusinessNameVi { get; set; } = "";
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<AlcoholBusinessDetailModel> AlcoholBusinessDetail { get; set; } = new List<AlcoholBusinessDetailModel>();
        public string GiayDangKyKinhDoanh { get; set; } = "";
        public DateTime? NgayCapPhep { get; set; }
        public string GiayPhepBanBuon { get; set; } = "";
        public DateTime? NgayCapGiayPhepBanBuon { get; set; }
        public DateTime? NgayHetHanGiayPhepBanBuon { get; set; }
    }
    public class removeListAlcoholBusinessItems
    {
        public List<Guid> AlcoholBusinessIds { get; set; }
    }
    public partial class AlcoholBusinessDetailModel
    {
        public string TenDoanhNghiep { get; set; }
        public string NguoiDaiDien { get; set; }
        public string SoDienThoai { get; set; }
        public Guid Huyen { get; set; }
        public Guid Xa { get; set; }
        public string? TenHuyen { get; set; }
        public string? TenXa { get; set; }
        public string? DiaChi { get; set; }
        public string? GiayPhepKinhDoanh { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string? DonViCungCap { get; set; }
        public string? DiaChiDonViCungCap { get; set; }
        public string? SoDienThoaiDonViCungCap { get; set; }
        public DateTime? NgayCapGiayPhepBanLe { get; set; }
        public string? GhiChu { get; set; }
        /// <summary>
        /// 1: Đã xóa; 0: Chưa xóa
        /// </summary>
        public bool IsDel { get; set; }
    }
}

