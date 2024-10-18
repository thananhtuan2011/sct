namespace API_SoCongThuong.Models
{
    public class CigaretteBusinessModel
    {
        public Guid CigaretteBusinessId { get; set; }
        public Guid CigaretteBusinessName { get; set; }
        public string? Supplier { get; set; } = "";
        public string Representative { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Address { get; set; } = "";
        public string BusinessNameVi { get; set; } = "";
        public bool IsDel { get; set; }
        public List<CigaretteBusinessDetailModel> CigaretteBusinessDetail { get; set; } = new List<CigaretteBusinessDetailModel>();
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string? GiayDangKyKinhDoanh { get; set; }
        public DateTime? NgayCap { get; set; }
    }
    public class removeListCigaretteBusinessItems
    {
        public List<Guid> CigaretteBusinessIds { get; set; }
    }

    public partial class CigaretteBusinessDetailModel
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
        public DateTime? NgayCap { get; set; }
        public string? DonViCungCap { get; set; }
        public string? DiaChiDonViCungCap { get; set; }
        public string? PhoneDonViCungCap { get; set; }
        public string? GhiChu { get; set; }

        /// <summary>
        /// 1: Đã xóa; 0: Chưa xóa
        /// </summary>
        public bool IsDel { get; set; }
    }
}
