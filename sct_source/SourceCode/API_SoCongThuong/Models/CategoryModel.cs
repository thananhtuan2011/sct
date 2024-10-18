namespace API_SoCongThuong.Models
{
    public partial class ConfigGroupModel
    {
        public Guid CategoryTypeId { get; set; }
        public string CategoryTypeCode { get; set; } = null!;
        public string CategoryTypeName { get; set; } = null!;
        public string? Description { get; set; }
    }

    public partial class ConfigModel
    {
        public Guid CategoryId { get; set; }
        public string CategoryTypeCode { get; set; } = null!;
        public string CategoryCode { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public int Priority { get; set; }
        public bool IsAction { get; set; }
        public bool IsDel { get; set; }
    }

    public partial class ListConfigModel
    {
        public List<ConfigModel> ListConfig { get; set; } = null!;
    }
}
