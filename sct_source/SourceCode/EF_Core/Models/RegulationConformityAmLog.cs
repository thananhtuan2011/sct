using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class RegulationConformityAmLog
    {
        public Guid LogId { get; set; }
        /// <summary>
        /// Id của công bố
        /// </summary>
        public Guid ItemId { get; set; }
        /// <summary>
        /// Id người dùng thực hiện thay đổi
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Thời gian thay đổi
        /// </summary>
        public DateTime LogTime { get; set; }
        /// <summary>
        /// Tên trường thay đổi
        /// </summary>
        public string Property { get; set; } = null!;
        /// <summary>
        /// Giá trị cũ
        /// </summary>
        public string OldValue { get; set; } = null!;
        /// <summary>
        /// Giá trị mới
        /// </summary>
        public string NewValue { get; set; } = null!;
    }
}
