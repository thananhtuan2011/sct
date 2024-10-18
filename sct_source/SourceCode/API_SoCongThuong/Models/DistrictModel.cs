using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API_SoCongThuong.Models
{
    public class DistrictModel
    {
        public Guid DistrictId { get; set; }
        public string DistrictCode { get; set; } = "";
        public string DistrictName { get; set; } = "";
        public int? CommuneNumber { get; set; }
        public bool IsDel { get; set; }
    }

    public class removeListDistrictItems
    {
        public List<Guid> DistrictIds { get; set; }
    }
}
