namespace API_SoCongThuong.Models
{
    public class ManageConfirmPromotionModel
    {
        public Guid ManageConfirmPromotionId { get; set; }
        public string ManageConfirmPromotionName { get; set; } = null!;
        public string GoodsServices { get; set; } = null!;
        public string? GoodsServicesPay { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string TimeStartGet { get; set; } = "";
        public string TimeEndGet { get; set; } = "";
        public string NumberOfDocuments { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<ManageConfirmPromotionAttachFileModel> Details { get; set; } = new List<ManageConfirmPromotionAttachFileModel>();
        public string IdFiles { get; set; } = "";
        public string LinkFileDisplay { get; set; } = "";

        public string InputDataPerson { get; set; } = "";

        public List<MCPInputDataPersonModel> ListInputDataPerson { get; set; } = new List<MCPInputDataPersonModel>();
    }
    public class removeListManageConfirmPromotionItems
    {
        public List<Guid> ManageConfirmPromotionId { get; set; }
    }

    public partial class ManageConfirmPromotionAttachFileModel
    {
        public Guid ManageConfirmPromotionAttachFileId { get; set; }

        public Guid ManageConfirmPromotionId { get; set; }

        public string LinkFile { get; set; } = null!;
    }

    public partial class MCPInputDataPersonModel
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
    }
}