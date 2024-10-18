using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TradeFairOrganizationCertificationAttachFile
    {
        /// <summary>
        /// File đính kèm văn bản xác nhận - Quản lý xác nhận tổ chức hội chợ triển lãm thương mại - Xúc tiến thương mại
        /// </summary>
        public Guid TradeFairFileId { get; set; }
        public Guid TradeFairOrganizationCertificationId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
