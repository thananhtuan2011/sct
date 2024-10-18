using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TrainingClassManagementAttachFile
    {
        public Guid TrainingClassManagementAttachFileId { get; set; }
        public Guid TrainingClassManagementId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
