using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ParticipateSupportFairDetail
    {
        public Guid ParticipateSupportFairDetailId { get; set; }
        public Guid ParticipateSupportFairId { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessCode { get; set; } = null!;
        public string BusinessNameVi { get; set; } = null!;
        public string NganhNghe { get; set; } = null!;
        public string DiaChi { get; set; } = null!;
        public string NguoiDaiDien { get; set; } = null!;
        public string? Huyen { get; set; }
        public string? Xa { get; set; }
    }
}
