namespace API_SoCongThuong.Models
{
    public class StageModel
    {
        public Guid StageId { get; set; }
        public string StageName { get; set; } = null!;
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}