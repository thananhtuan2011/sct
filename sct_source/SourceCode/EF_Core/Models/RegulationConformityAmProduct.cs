using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class RegulationConformityAmProduct
    {
        /// <summary>
        /// ID sản phẩm
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// ID Công bố hợp quy
        /// </summary>
        public Guid RegulationConformityAmid { get; set; }
        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string ProductName { get; set; } = null!;
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
    }
}
