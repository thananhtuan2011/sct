
namespace API_SoCongThuong.Models
{
    public class ParticipateTradePromotionModel
    {
        public Guid ParticipateSupportFairDetailId { get; set; }
        public Guid? ParticipateSupportFairId { get; set; }
        public string? ParticipateSupportFairName { get; set; }
        public string CountryName { get; set; } = "";
        // Info Business
        public Guid? BusinessId { get; set; }
        public string TenGiaoDich { get; set; } = "";
        public string BusinessNameEn { get; set; } = "";
        public string DiaChiTruSo { get; set; } = "";
        public DateTime? NgayCapPhep { get; set; }
        public string MaSoThue { get; set; } = "";
        public string BusinessNameVi { get; set; } = "";
        public Guid? LoaiNganhNghe { get; set; }
        //Name loai nganh nghe
        public string TypeOfProfessionName { get; set; } = "";
        public DateTime? NgayHoatDong { get; set; }
        public string NguoiDaiDien { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string GiamDoc { get; set; } = "";
        public string Email { get; set; } = "";
        public string BusinessCode { get; set; } = "";
        public string Industrial { get; set; } = "";
        public List<ParticipateTradePromotionDetailModel> Detail1s { get; set; } = new List<ParticipateTradePromotionDetailModel>();
        public List<ParticipateTradePromotionDetail2Model> Detail2s { get; set; } = new List<ParticipateTradePromotionDetail2Model>();
        public List<ParticipateTradePromotionDetailModel> Detail3s { get; set; } = new List<ParticipateTradePromotionDetailModel>();
    }

    public partial class ParticipateTradePromotionDetailModel
    {
        public Guid ParticipateSupportFairId { get; set; }
        public Guid? Country { get; set; } = null!;
        public string ParticipateSupportFairName { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Address { get; set; } = null!;
        public int PlanJoin { get; set; } = 0;
        public string Scale { get; set; } = null!;
        public bool IsDel { get; set; } = false;
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public string CapitalName { get; set; } = "";
        public long Funding { get; set; } = 0;
    }

    public partial class ParticipateTradePromotionDetail2Model
    {
        public Guid BusinessId { get; set; }
        public string IndustryName { get; set; } = null!;
        public string IndustryCode { get; set; } = null!;
        public bool IsDel { get; set; } = false;
    }
}
