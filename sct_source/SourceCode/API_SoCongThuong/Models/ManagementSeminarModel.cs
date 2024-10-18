namespace API_SoCongThuong.Models
{
    public partial class ManagementSeminarModel
    {
        public Guid ManagementSeminarId { get; set; }
        public string ProfileCode { get; set; } = null!;
        public Guid BusinessId { get; set; }
        public string Title { get; set; } = null!;
        public Guid DistrictId { get; set; }
        public string? Address { get; set; }
        public string? Contact { get; set; }
        public string? PhoneNumber { get; set; }
        public int NumberParticipant { get; set; }
        public string? Note { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string BusinessName { get; set; } = "";
        public List<TimeManagementSeminarModel> listTime { get; set; } = new List<TimeManagementSeminarModel>();
    }

    public partial class TimeManagementSeminarModel
    {
        public Guid ManagementSeminarId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
