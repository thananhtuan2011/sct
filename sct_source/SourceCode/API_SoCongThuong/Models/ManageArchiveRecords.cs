namespace API_SoCongThuong.Models
{
    public class ManageArchiveRecordsModel
    {
        public Guid ManageArchiveRecordsId { get; set; }
        public int RecordsFinancePlanId { get; set; }
        public string? RecordsFinancePlan { get; set; }
        public string CodeFile { get; set; } = null!;
        public string Title { get; set; } = null!;
        public DateTime ReceptionTime { get; set; }
        public int StorageTime { get; set; }
        public string Location { get; set; } = null!;
        public string StoreDocumentsAt { get; set; } = null!;
        public string StoreFilesAt { get; set; } = null!;
        public string Creator { get; set; } = null!;
        public string? Note { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
