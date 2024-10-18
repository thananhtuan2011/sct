namespace API_SoCongThuong.Models
{
    public class GasModel
    {
        public Guid GasId { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
