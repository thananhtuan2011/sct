namespace API_SoCongThuong.Models
{
    public class ManagementElectricityActivitiesMonthReportModel
    {
        public Guid MonthReportId { get; set; }
        public string UpdateDate { get; set; } = null!;
        public int Month { get; set; }
        public int Year { get; set; }
        public List<MonthReportAttchFileModel>? AllFile { get; set; } = new List<MonthReportAttchFileModel>();
        public string DelFileIds { get; set; } = "";
        public Guid? CreateUserId { get; set; }
        public DateTime? CreateTime { get; set; }
    }

    public class MonthReportAttchFileModel
    {
        public Guid FileId { get; set; }
        public Guid MonthReportId { get; set; }
        public string? LinkFile { get; set; }
        public string? CreateTime { get; set;}
    }
}