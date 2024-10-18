using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TestGuidManagementAttachFile
    {
        public Guid TestGuidManagementAttachFileId { get; set; }
        public Guid TestGuidManagementId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
