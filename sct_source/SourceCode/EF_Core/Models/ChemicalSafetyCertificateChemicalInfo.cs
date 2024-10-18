using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ChemicalSafetyCertificateChemicalInfo
    {
        /// <summary>
        /// Bảng thôn tin hóa chất - giấy chứng nhận - An toàn hóa chất
        /// </summary>
        public Guid ChemicalId { get; set; }
        /// <summary>
        /// Id giấy chứng nhận
        /// </summary>
        public Guid ChemicalSafetyCertificateId { get; set; }
        /// <summary>
        /// Tên thương mại
        /// </summary>
        public string TradeName { get; set; } = null!;
        /// <summary>
        /// Tên hóa chất
        /// </summary>
        public string NameOfChemical { get; set; } = null!;
        /// <summary>
        /// Mã CAS
        /// </summary>
        public string? Cascode { get; set; }
        /// <summary>
        /// Công thức hóa học
        /// </summary>
        public string? ChemicalFormula { get; set; }
        /// <summary>
        /// Hàm lượng
        /// </summary>
        public string? Content { get; set; }
        /// <summary>
        /// Khối lượng (Tấn/Năm)
        /// </summary>
        public string? Mass { get; set; }
    }
}
