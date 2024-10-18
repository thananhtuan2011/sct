namespace API_SoCongThuong.Models
{
    public class RecordsManagerModel
    {
        public Guid RecordsManagerId { get; set; }
        public Guid RecordsFinancePlanId { get; set; }
        public string? RecordsFinancePlan { get; set; }
        public string CodeFile { get; set; } = "";
        public string Title { get; set; } = "";
        public DateTime ReceptionTime { get; set; }
        public decimal StorageTime { get; set; } = 0;
        public string Creator { get; set; } = "";
        public string? Note { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
