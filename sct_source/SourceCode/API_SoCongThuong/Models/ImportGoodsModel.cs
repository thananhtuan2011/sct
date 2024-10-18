namespace API_SoCongThuong.Models
{
    public class ImportGoodsModel
    {
        public Guid ImportGoodsId { get; set; }
        public string ImportGoodsName { get; set; } = null!;
        public Guid ItemGroupId { get; set; }
        public Guid TypeOfEconomicId { get; set; }
        public Guid BusinessId { get; set; }
        public Guid CountryId { get; set; }
        public decimal Amount { get; set; }
        public Guid AmountUnit { get; set; }
        public decimal Price { get; set; }
        public DateTime ImportTime { get; set; }
        public bool IsDel { get; set; }
    }
    public class GetImportGoodsModel
    {
        public Guid ImportGoodsId { get; set; }
        public string ImportGoodsName { get; set; } = null!;
        public Guid ItemGroupId { get; set; }
        public Guid TypeOfEconomicId { get; set; }
        public Guid BusinessId { get; set; }
        public Guid CountryId { get; set; }
        public decimal Amount { get; set; }
        public Guid AmountUnitId { get; set; }
        public decimal Price { get; set; }
        public string ImportTime { get; set; } = null!;
        public bool IsDel { get; set; }
    }
    public class removeListImportGoodsItems
    {
        public List<Guid> ImportGoodsIds { get; set; }
    }
    public class ImportGoodsView
    {
        public Guid ImportGoodsId { get; set; }
        public string ImportGoodsName { get; set; } = null!;
        public string ItemGroupName { get; set; } = null!;
        public string TypeOfEconomicName { get; set; } = null!;
        //public string BusinessName { get; set; } = null!;
        public string CountryName { get; set; } = null!;
        public string Amount { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImportTime { get; set; } = null!;
        public DateTime? ImportTimeDate { get; set; }
    }
    public class ItemGroup
    {
        public Guid ItemGroupId { get; set; }
        public string ItemGroupName { get; set; } = null!;
    }
    public class TypeOfEconomic
    {
        public Guid TypeOfEconomicId { get; set; }
        public string TypeOfEconomicName { get; set; } = null!;
    }
    public class CountryView
    {
        public Guid CountryId { get; set; }
        public string CountryName { get; set; } = null!;
    }

    public class BusinessView
    {
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = null!;
    }

    public class UnitsView
    {
        public Guid UnitId { get; set; }
        public string UnitName { get; set; } = null!;
    }
}