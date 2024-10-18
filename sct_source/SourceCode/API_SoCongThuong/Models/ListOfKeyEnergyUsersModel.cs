namespace API_SoCongThuong.Models
{
    public class ListOfKeyEnergyUsersModel
    {
        public Guid ListOfKeyEnergyUsersId { get; set; }
        public Guid BusinessId { get; set; }
        public string? BusinessName { get; set; }
        public string Address { get; set; } = null!;
        public int? Date { get; set; }
        public DateTime? DateOnly { get; set; }
        public string Link { get; set; } = null!;
        public bool IsDel { get; set; }
        public Guid? Profession { get; set; }
        public string? ManufactProfession { get; set; }
        public decimal EnergyConsumption { get; set; } = 0;
        public string? Note { get; set; }
        public string DistrictName { get; set; } = "";
        public string ProfessionName { get; set; } = "";
        public Guid? District { get; set; } = Guid.Empty;
        public string Decision { get; set; } = "";
    }
}