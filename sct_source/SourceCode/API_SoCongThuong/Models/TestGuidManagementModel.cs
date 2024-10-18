namespace API_SoCongThuong.Models
{
    public class TestGuidManagementModel
    {
        public Guid TestGuidManagementId { get; set; }
        public string InspectionAgency { get; set; } = "";
        public DateTime Time { get; set; }
        public string TimeGet { get; set; } = "";
        public string CoordinationAgency { get; set; } = "";
        public string Result { get; set; } = "";
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<TestGuidManagementAttachFileModel> Details { get; set; } = new List<TestGuidManagementAttachFileModel>();
        public string IdFiles { get; set; } = "";
        public string LinkFileDisplay { get; set; } = "";
    }
    public partial class TestGuidManagementAttachFileModel
    {
        public Guid TestGuidManagementAttachFileId { get; set; }

        public Guid TestGuidManagementId { get; set; }

        public string LinkFile { get; set; } = null!;
    }
}
