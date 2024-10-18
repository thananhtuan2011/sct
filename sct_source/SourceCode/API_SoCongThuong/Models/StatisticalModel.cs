using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace API_SoCongThuong.Models
{
    public class StatisticalByProvinceModel
    {
        public List<StatisticalByProvinceDetailModel> Details { get; set; } = new List<StatisticalByProvinceDetailModel>();
    }

    public class StatisticalByCommuneModel
    {
        public List<StatisticalByCommuneDetailModel> Details { get; set; } = new List<StatisticalByCommuneDetailModel>();
        public TotalStatisticalByProvinceDetailModel Total { get; set; } = new TotalStatisticalByProvinceDetailModel();
    }

    public partial class StatisticalByProvinceDetailModel
    {
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int SLTrungTamThuongMai { get; set; } = 0;
        public int SLSieuThi { get; set; } = 0;
        public int SLChoTrongQuyHoach { get; set; } = 0;
        public int SLChoNgoaiQuyHoach { get; set; } = 0;
        public int SLChoDem { get; set; } = 0;
        public int SLChoNoi { get; set; } = 0;
        public int SLCuaHangTienLoi { get; set; } = 0;
        public int SLCuaHangTapHoa { get; set; } = 0;
        public int SLCuaHangChuyenDoanh { get; set; } = 0;
        public int SLTrungTamLogictis { get; set; } = 0;
        public int Total { get; set; } = 0;
    }

    public partial class StatisticalByCommuneDetailModel
    {
        public Guid MaHuyen { get; set; }
        public string TenHuyen { get; set; }
        public Guid MaXa { get; set; }
        public string TenXa { get; set; }
        public Guid MaCho { get; set; }
        public string TenCho { get; set; }
        public string LoaiHinh { get; set; }
        public int SoSap { get; set; } = 0;
        public int SoNganhHangKinhDoanh { get; set; } = 0;
    }

    public partial class TotalStatisticalByProvinceDetailModel
    {
        public int KhuVuc { get; set; } = 0;
        public int LoaiHinh { get; set; } = 0;
        public int SoSap { get; set; } = 0;
        public int SoNganhHangKinhDoanh { get; set; } = 0;
    }

    public class QueryStatisticalBody
    {
        [JsonPropertyName("filter")]//for derelize of NEWTON.JSON
        [JsonProperty("filter")]//for serilize of NEWTON.JSON
        public Dictionary<string, string> Filter { get; set; }
    }

    // Thống kê nâng cấp, xây dựng chợ, ST, TTTM
    public class StatisticalHasBeenBuildUpgradedModel
    {
        public List<StatisticalHasBeenBuildUpgradedDetailModel> Details { get; set; } = new List<StatisticalHasBeenBuildUpgradedDetailModel>();
        public TotalStatisticalHasBeenBuildUpgradedDetailModel Total { get; set; } = new TotalStatisticalHasBeenBuildUpgradedDetailModel();
    }

    public partial class StatisticalHasBeenBuildUpgradedDetailModel
    {
        public Guid MaHuyen { get; set; }
        public string TenHuyen { get; set; }
        public int TongCho { get; set; } = 0;
        public decimal TongVonDautu { get; set; } = 0;
        public decimal VonDaThucHien { get; set; } = 0;
        public decimal VonNganSach { get; set; } = 0;
        public decimal VonCQSDDat { get; set; } = 0;
        public decimal VonVay { get; set; } = 0;
        public decimal VonKhac { get; set; } = 0;
    }

    public partial class TotalStatisticalHasBeenBuildUpgradedDetailModel
    {
        public int TongCho { get; set; } = 0;
        public decimal TongVonDautu { get; set; } = 0;
        public decimal VonDaThucHien { get; set; } = 0;
        public decimal VonNganSach { get; set; } = 0;
        public decimal VonCQSDDat { get; set; } = 0;
        public decimal VonVay { get; set; } = 0;
        public decimal VonKhac { get; set; } = 0;
    }

    public class StatisticalHasNotBuildUpgradedDetailModel
    {
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int TongCho { get; set; } = 0;
    }

    public class StatisticalHasNotBuildUpgradedByIdDetailModel
    {
        public Guid CommercialId { get; set; }
        public string TenCho { get; set; }
        public string KhuVuc { get; set; }
        public string LoaiHinh { get; set; }
        public int SoSap { get; set; } = 0;
        public int SoNganhHangKinhDoanh { get; set; } = 0;
    }

    public class StatisticalMarketModel
    {
        public string TenCho { get; set; }
        public string Huyen { get; set; }
        public decimal GiaTrongNhaLong { get; set; } = 0;
        public decimal GiaNgoaiNhaLong { get; set; } = 0;
        public decimal DeXuatGiaMoi { get; set; } = 0;
    }

    public class StatisticalBusinessStoreModel
    {
        public string TenDoanhNghiep { get; set; }
        public string Huyen { get; set; }
        public string NguoiDaiDien { get; set; }
        public string SoDienThoai { get; set; }
        public List<BusinessStoreDetailModel> QuanLyXangDau { get; set; } = new List<BusinessStoreDetailModel>();
        public List<BusinessStoreDetailModel> QuanLyThuocLa { get; set; } = new List<BusinessStoreDetailModel>();
        public List<BusinessStoreDetailModel> QuanLyRuou { get; set; } = new List<BusinessStoreDetailModel>();
        
    }

    public partial class BusinessStoreDetailModel
    {
        public string TenDoanhNghiep { get; set; }
        public string Huyen { get; set; }
        public string NguoiDaiDien { get; set; }
        public string SoDienThoai { get; set; }
        public Guid BusinessId { get; set; } = Guid.Empty;
    }

    public partial class BusinessDetailModel
    {
        public Guid? CigaretteBusinessId { get; set; }
        public Guid? PetroleumBusinessId { get; set; }
        public Guid? AlcoholBusinessId { get; set; }
        public Guid BusinessId { get; set; }
        public string TenDoanhNghiep { get; set; }
        public string Huyen { get; set; }
        public string NguoiDaiDien { get; set; }
        public string SoDienThoai { get; set; }
    }
    public class StatisticalProductAlcolDetailModel
    {
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int TongDoanhNghiep { get; set; } = 0;
    }
    public class StatisticalIndusAlcolDetailModel
    {
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int TongDoanhNghiep { get; set; } = 0;
    }

    public class StatisticalFairParticipantModel
    {
        public Guid ParticipateSupportFairId { get; set; }
        public string TenChuongTrinh { get; set; }
        public Guid? DistrictId { get; set; }
        public string Huyen { get; set; }
        public string DuKienToChuc { get; set; }
        public DateTime ThoiGianToChuc { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string DuKienKetThuc { get; set; }
        public string QuyMo { get; set; }
        public int SoLuongDoanhNghiep { get; set; }
    }
}
