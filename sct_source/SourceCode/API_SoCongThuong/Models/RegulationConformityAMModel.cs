//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API_SoCongThuong.Models
{
    public class RegulationConformityAMModel
    {
        public Guid RegulationConformityAMId { get; set; }
        public string DayReception { get; set; } = null!;
        public DateTime? DayReceptionDate { get; set; }
        public Guid EstablishmentId { get; set; }
        public string? EstablishmentName { get; set; }
        public Guid DistrictId { get; set; }
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Num { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string DateOfPublication { get; set; } = null!;
        public string? Note { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }

    public class RegulationConformityAmLogModel
    {
        public Guid LogId { get; set; }
        public Guid ItemId { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime LogTime { get; set; }
        public string? LogTimeDisplay { get; set; }
        public string? Property { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
