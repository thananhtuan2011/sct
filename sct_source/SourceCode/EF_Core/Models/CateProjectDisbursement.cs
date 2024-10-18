using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateProjectDisbursement
    {
        public Guid CateProjectDisbursementId { get; set; }
        public Guid CateProjectId { get; set; }
        public DateTime DisbursementDate { get; set; }
        public long DisbursementMoney { get; set; }
        public Guid DisbursementUnits { get; set; }
        public bool IsConfirm { get; set; }
    }
}
