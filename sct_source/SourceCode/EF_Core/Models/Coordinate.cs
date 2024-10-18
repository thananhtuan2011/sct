using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class Coordinate
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Id của Item cần lưu tọa độ
        /// </summary>
        public Guid ItemId { get; set; }
        /// <summary>
        /// Bảng lưu Item
        /// </summary>
        public string TableCode { get; set; } = null!;
        /// <summary>
        /// Địa chỉ của Item
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Vĩ độ của Item
        /// </summary>
        public decimal Lat { get; set; }
        /// <summary>
        /// Tung độ của Item
        /// </summary>
        public decimal Lng { get; set; }
        /// <summary>
        /// Địa chỉ của Geocode
        /// </summary>
        public string FormattedAddress { get; set; } = null!;
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Icon hiển thị
        /// </summary>
        public string Icon { get; set; } = null!;
    }
}
