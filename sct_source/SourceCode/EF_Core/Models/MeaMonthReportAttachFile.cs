using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class MeaMonthReportAttachFile
    {
        /// <summary>
        /// Id File Văn bản liên quan
        /// </summary>
        public Guid FileId { get; set; }
        /// <summary>
        /// Id Tháng báo cáo
        /// </summary>
        public Guid MonthReportId { get; set; }
        /// <summary>
        /// Url của File
        /// </summary>
        public string LinkFile { get; set; } = null!;
        public DateTime CreateTime { get; set; }
    }
}
