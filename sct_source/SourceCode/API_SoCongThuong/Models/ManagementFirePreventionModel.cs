namespace API_SoCongThuong.Models
{
    public class ManagementFirePreventionModel
    {
        public Guid ManagementFirePreventionId { get; set; }
        public string BusinessName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int Reality { get; set; }
        public string RealityName { get; set; } = "";
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}