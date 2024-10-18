using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ChemicalSafetyCertificateAttachFile
    {
        public Guid ChemicalSafetyCertificateAttachFileId { get; set; }
        public Guid ChemicalSafetyCertificateId { get; set; }
        public string LinkFile { get; set; } = null!;
    }
}
