namespace API_SoCongThuong.Models
{
    public class TrainingClassManagementModel
    {
        public Guid TrainingClassManagementId { get; set; }
        public string Topic { get; set; } = "";
        public DateTime TimeStart { get; set; }
        public string TimeStartGet { get; set; } = "";
        public string Location { get; set; } = "";
        public string Participant { get; set; } = "";
        public int NumberOfAttendees { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<TrainingClassManagementAttachFileModel> Details { get; set; } = new List<TrainingClassManagementAttachFileModel>();
        public string IdFiles { get; set; } = "";
        public string LinkFileDisplay { get; set; } = "";
    }
    public partial class TrainingClassManagementAttachFileModel
    {
        public Guid TrainingClassManagementAttachFileId { get; set; }

        public Guid TrainingClassManagementId { get; set; }

        public string LinkFile { get; set; } = null!;
    }
}
