namespace API_SoCongThuong.Models
{
    public class ImportExportInfoModel
    {
        public Guid ItemId { get; set; }
        public string GoodsName { get; set; } = null!;
        public string BusinessName { get; set; } = null!;
        public string CountryName { get; set; } = null!;
        public string Amount { get; set; } = null!;
        public string Price { get; set; } = null!;
        public string Time { get; set; } = null!;
        public string Method { get; set; } = null!;
    }
    public class CriteriaModel
    {
        public string Amount { get; set; } = null!;
        public string Price { get; set; } = null!;
    }
    public class ItemsModel
    {
        public string ItemName { get; set; } = null!;
    }
}