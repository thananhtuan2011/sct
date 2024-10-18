using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TrainingManagementAttachFile
    {
        public Guid TrainingManagementAttachFileId { get; set; }
        public Guid TrainingManagementId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
