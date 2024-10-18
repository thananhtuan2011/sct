namespace API_SoCongThuong.Models
{
    public class TradePromotionProjectManagementModel
    {
        public Guid TradePromotionProjectManagementId { get; set; }
        public string TradePromotionProjectManagementName { get; set; } = "";
        public string? ImplementingAgencies { get; set; }
        public decimal? Cost { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public string TimeStartGet { get; set; } = "";
        public string TimeEndGet { get; set; } = "";
        public int NumberOfApprovalDocuments { get; set; }
        public byte? ImplementationResults { get; set; }
        public byte? Status { get; set; }
        public string? Reason { get; set; }
        public bool IsDel { get; set; }

        public DateTime CreateTime { get; set; }
  
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<TradePromotionProjectManagementAttachFileModel> Details { get; set; } = new List<TradePromotionProjectManagementAttachFileModel>();
        public string IdFiles { get; set; } = "";
        public string LinkFileDisplay { get; set; } = "";

        public string BusinessDetail { get; set; } = "";

        public List<TradePromotionProjectManagementDetailModel> BusinessDetails { get; set; } = new List<TradePromotionProjectManagementDetailModel>();

        public string InputDataPerson { get; set; } = "";

        public List<InputDataPersonModel> ListInputDataPerson { get; set; } = new List<InputDataPersonModel>();
    }
    public class removeListTradePromotionProjectManagementItems
    {
        public List<Guid> TradePromotionProjectManagementIds { get; set; }
    }

    public partial class TradePromotionProjectManagementAttachFileModel
    {
        public Guid TradePromotionProjectManagementAttachFileId { get; set; }

        public Guid TradePromotionProjectManagementId { get; set; }

        public string LinkFile { get; set; } = null!;
    }

    public partial class TradePromotionProjectManagementDetailModel
    {
        public Guid TradePromotionProjectManagementDetailId { get; set; }
        public Guid TradePromotionProjectManagementId { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessCode { get; set; } = null!;
        public string BusinessNameVi { get; set; } = null!;
        public string NganhNghe { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public string NguoiDaiDien { get; set; } = null!;
    }

    public partial class InputDataPersonModel
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
    }
}