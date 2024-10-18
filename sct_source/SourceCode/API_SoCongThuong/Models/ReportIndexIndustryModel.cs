using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace API_SoCongThuong.Models
{
    public class ReportIndexIndustryModel
    {
        public Guid ReportIndexIndustryId { get; set; }
        public byte Target { get; set; } = 0;
        public int MonthX { get; set; } = 0;
        public int YearX { get; set; } = 0; 
        public string Month { get; set; } = null!;
        public List<ValueDataTarget>? DataTarget { get; set; } = new List<ValueDataTarget>();
        public decimal? ComparedPreviousMonth { get; set; }
        public decimal? SamePeriod { get; set; }
        public decimal? Accumulation { get; set; }
        public decimal ReportIndex { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public string TargetName { get; set; } = "";
    }

    public class ValueDataTarget
    {
        public int Month { get; set; }
        public decimal ReportIndex { get; set; }
        public Guid ReportIndexIndustryId { get; set; } = Guid.Empty;

    }

    public class DataTarget
    {
        public byte Target { get; set; }
        public decimal ReportIndex { get; set; }
    }
}
