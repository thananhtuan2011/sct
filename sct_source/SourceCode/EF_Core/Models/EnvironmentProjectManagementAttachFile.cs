using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class EnvironmentProjectManagementAttachFile
    {
        public Guid EnvironmentProjectManagementAttachFileId { get; set; }
        public Guid EnvironmentProjectManagementId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
