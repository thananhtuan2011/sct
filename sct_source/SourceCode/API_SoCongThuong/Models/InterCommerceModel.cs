namespace API_SoCongThuong.Models
{
    public class InterCommerceModel
    {
        public Guid InternationalCommerceId { get; set; }
        public Guid InternationalCommerceName { get; set; }
        public string InvestorName { get; set; } = "";
        public string Address { get; set; } = "";
        public string LicensingActivity { get; set; } = "";
        public string Representative { get; set; } = "";
        public string BusinessNameVi { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
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
        public bool IsDel { get; set; }
        public string TenLoaiHinhCoSo { get; set; } = "";
    }
    public class removeListInterCommerceItems
    {
        public List<Guid> InternationalCommerceIds { get; set; }
    }
}