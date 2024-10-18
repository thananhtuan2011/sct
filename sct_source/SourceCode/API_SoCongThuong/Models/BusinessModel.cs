using EF_Core.Models;

namespace API_SoCongThuong.Models
{
    public class BusinessModel
    {
        public Guid BusinessId { get; set; }
        //Thông tin doanh nghiệp
        public string BusinessCode { get; set; } = null!;
        public string? MaSoThue { get; set; }
        public string TenGiaoDich { get; set; } = null!;
        public string BusinessNameVi { get; set; } = null!;
        public string? BusinessNameEn { get; set; }
        public Guid LoaiHinhDoanhNghiep { get; set; }
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
        public string? DiaChiTruSo { get; set; }
        public Guid? LoaiNganhNghe { get; set; }
        public DateTime? NgayCapPhep { get; set; }
        public DateTime? NgayHoatDong { get; set; }
        public string? GiayPhepSanXuat { get; set; }
        //Thông tin liên lạc
        public string? NguoiDaiDien { get; set; }
        public string? SoDienThoai { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? Cccd { get; set; }
        public string? NgayCap { get; set; }
        public string? NoiCap { get; set; }
        public string? DiaChi { get; set; }
        public string? GiamDoc { get; set; }
        public string? Email { get; set; }
        //Ngành nghề
        public string? IndustryIdString { get; set; }
        public List<Guid>? IndustryId { get; set; }
        //Logo
        public List<BusinessLogoModel> Details { get; set; } = new List<BusinessLogoModel>();
        public string IdFiles { get; set; } = "";
        public string LinkFileDisplay { get; set; } = "";
        public bool IsDel { get; set; }
    }

    public class GetBusinessModel
    {
        public Guid BusinessId { get; set; }
        //Thông tin doanh nghiệp
        public string BusinessCode { get; set; } = null!;
        public string? MaSoThue { get; set; }
        public string TenGiaoDich { get; set; } = null!;
        public string BusinessNameVi { get; set; } = null!;
        public string? BusinessNameEn { get; set; }
        public Guid LoaiHinhDoanhNghiep { get; set; }
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
        public string? DiaChiTruSo { get; set; }
        public Guid? LoaiNganhNghe { get; set; }
        public string? NgayHoatDong { get; set; }
        public string? NgayCapPhep { get; set; }
        public string? GiayPhepSanXuat { get; set; }
        //Thông tin liên lạc
        public string? NguoiDaiDien { get; set; }
        public string? SoDienThoai { get; set; }
        public string? NgaySinh { get; set; }
        public string? Cccd { get; set; }
        public string? NgayCap { get; set; }
        public string? NoiCap { get; set; }
        public string? DiaChi { get; set; }
        public string? GiamDoc { get; set; }
        public string? Email { get; set; }
        //Ngành nghề
        public string? IndustryIdString { get; set; }
        public List<Guid>? IndustryId { get; set; }
        //Logo
        public List<BusinessLogoModel> Details { get; set; } = new List<BusinessLogoModel>();
        public string IdFiles { get; set; } = "";
        public string LinkFileDisplay { get; set; } = "";
        //Xóa
        public bool IsDel { get; set; }
    }

    public class removeListBusinessItems
    {
        public List<Guid> BusinessIds { get; set; }
    }

    public class viewdata
    {
        public Business databusiness { get; set; }
        public string datatypeofbusiness { get; set; }
        public string? datatypeofprofession { get; set; }
        public List<listindustry>? dataindustry { get; set; }

    }

    public class listindustry
    {
        public string IndustryCode { get; set; }
        public string IndustryName { get; set; }
    }

    public class NbgDate
    {
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
    }

    public partial class BusinessLogoModel
    {
        public Guid LogoId { get; set; }
        public Guid BusinessId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}