namespace API_SoCongThuong.Models
{
    public class GasTrainingClassManagementModel
    {
        public Guid GasTrainingClassManagementId { get; set; }
        public string Topic { get; set; } = null!;
        public DateTime TimeStart { get; set; }
        public string TimeStartGet { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Participant { get; set; } = null!;
        public int NumberOfAttendees { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<GasTrainingClassManagementAttachFileModel> Details { get; set; } = new List<GasTrainingClassManagementAttachFileModel>();
        public string? IdFiles { get; set; }
        public string? LinkFileDisplay { get; set; }
    }
    public partial class GasTrainingClassManagementAttachFileModel
    {
        public Guid GasTrainingClassManagementAttachFileId { get; set; }

        public Guid GasTrainingClassManagementId { get; set; }

        public string LinkFile { get; set; } = null!;
    }
}
