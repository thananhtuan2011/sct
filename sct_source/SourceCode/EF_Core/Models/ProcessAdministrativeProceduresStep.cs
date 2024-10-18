using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ProcessAdministrativeProceduresStep
    {
        /// <summary>
        /// Quy trình nội bộ giải quyết thủ tục hành chính - Các bước
        /// </summary>
        public Guid ProcessAdministrativeProceduresStepId { get; set; }
        /// <summary>
        /// Id quy trình nội bộ giải quyết thủ tục hành chính
        /// </summary>
        public Guid ProcessAdministrativeProceduresId { get; set; }
        /// <summary>
        /// Bước
        /// </summary>
        public int Step { get; set; }
        /// <summary>
        /// Đơn vị thực hiện
        /// </summary>
        public string ImplementingAgencies { get; set; } = null!;
        /// <summary>
        /// Thời gian thực hiện
        /// </summary>
        public decimal ProcessingTime { get; set; }
        /// <summary>
        /// Nội dung thực hiện
        /// </summary>
        public string? ContentImplementation { get; set; }
    }
}
