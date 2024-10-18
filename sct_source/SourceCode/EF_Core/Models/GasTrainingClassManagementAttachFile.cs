using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class GasTrainingClassManagementAttachFile
    {
        public Guid GasTrainingClassManagementAttachFileId { get; set; }
        public Guid GasTrainingClassManagementId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
