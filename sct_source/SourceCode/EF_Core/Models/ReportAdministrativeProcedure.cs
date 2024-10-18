using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ReportAdministrativeProcedure
    {
        /// <summary>
        /// Bảng thủ báo cáo tình hình giải quyết thủ tục hành chính - thủ tục hành chính - thanh tra sở
        /// </summary>
        public Guid ReportId { get; set; }
        /// <summary>
        /// Kỳ báo cáo
        /// </summary>
        public Guid Period { get; set; }
        /// <summary>
        /// Năm báo cáo
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Id lĩnh vực
        /// </summary>
        public Guid AdministrativeProceduresField { get; set; }
        /// <summary>
        /// Online trong kỳ
        /// </summary>
        public int OnlineInPeriod { get; set; }
        /// <summary>
        /// Offline trong kỳ
        /// </summary>
        public int OfflineInPeriod { get; set; }
        /// <summary>
        /// Từ kỳ trước
        /// </summary>
        public int FromPreviousPeriod { get; set; }
        /// <summary>
        /// Đúng hạn - đã giải quyết
        /// </summary>
        public int OnTimeProcessed { get; set; }
        /// <summary>
        /// Quá hạn - đã giải quyết
        /// </summary>
        public int OutOfDateProcessed { get; set; }
        /// <summary>
        /// Trước hạn - đã giải quyết
        /// </summary>
        public int BeforeDeadlineProcessed { get; set; }
        /// <summary>
        /// Trong hạng - đang xử lý
        /// </summary>
        public int OnTimeProcessing { get; set; }
        /// <summary>
        /// Quá hạn - đang xử lý
        /// </summary>
        public int OutOfDateProcessing { get; set; }
        /// <summary>
        /// 1: Hoạt động 0: Không hoạt động
        /// </summary>
        public bool? IsAction { get; set; }
        /// <summary>
        /// 1: Đã xóa; 0: Chưa xóa
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
