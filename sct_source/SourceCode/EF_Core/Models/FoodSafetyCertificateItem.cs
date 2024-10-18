using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class FoodSafetyCertificateItem
    {
        /// <summary>
        /// Id Item
        /// </summary>
        public Guid ItemId { get; set; }
        /// <summary>
        /// Id giấy chứng nhận
        /// </summary>
        public Guid FoodSafetyCertificateId { get; set; }
        /// <summary>
        /// Id loại hình
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// Tên loại hình
        /// </summary>
        public string ProductName { get; set; } = null!;
    }
}
