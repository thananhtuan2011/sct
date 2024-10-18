using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ManageConfirmPromotionAttachFile
    {
        public Guid ManageConfirmPromotionAttachFileId { get; set; }
        public Guid ManageConfirmPromotionId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
