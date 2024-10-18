using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class FoodSafetyCertificateAttachFile
    {
        public Guid FoodSafetyCertificateAttachFileId { get; set; }
        public Guid FoodSafetyCertificateId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
