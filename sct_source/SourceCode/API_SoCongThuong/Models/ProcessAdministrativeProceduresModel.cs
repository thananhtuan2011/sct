using EF_Core.Models;

namespace API_SoCongThuong.Models
{
    public class ProcessAdministrativeProceduresModel
    {
        /// Quy trình nội bộ giải quyết thủ tục hành chính - Thủ tục hành chính - Sở thanh tra
        public Guid ProcessAdministrativeProceduresId { get; set; }

        /// Lĩnh vực giải quyết
        public Guid ProcessAdministrativeProceduresField { get; set; }
        public string ProcessAdministrativeProceduresFieldName { get; set; } = "";

        /// Mã quy trình
        public string ProcessAdministrativeProceduresCode { get; set; } = null!;

        /// Tên quy trình
        public string ProcessAdministrativeProceduresName { get; set; } = null!;

        /// 1: Đã xóa; 0: Chưa xóa
        public bool IsDel { get; set; }

        public Guid CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }

        public List<ProcessAdministrativeProceduresStep> ProcessStep { get; set; }
        public decimal? TotalTimeProcess { get; set; } = 0;
    }
}