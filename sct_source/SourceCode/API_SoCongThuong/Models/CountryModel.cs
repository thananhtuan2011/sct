namespace API_SoCongThuong.Models
{
    public class CountryModel
    {
        public Guid CountryId { get; set; }
        public string CountryCode { get; set; } = "";
        public string CountryName { get; set; } = "";
        public bool IsDel { get; set; }
    }
}