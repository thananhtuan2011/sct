using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class RegulationConformityAm
    {
        public Guid RegulationConformityAmid { get; set; }
        /// <summary>
        /// Ngày tiếp nhận
        /// </summary>
        public DateTime DayReception { get; set; }
        /// <summary>
        /// Tên cơ sở
        /// </summary>
        public Guid EstablishmentId { get; set; }
        /// <summary>
        /// Id huyện
        /// </summary>
        public Guid DistrictId { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string Phone { get; set; } = null!;
        /// <summary>
        /// Số công bố
        /// </summary>
        public string Num { get; set; } = null!;
        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string ProductName { get; set; } = null!;
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; } = null!;
        /// <summary>
        /// Ngày công bố
        /// </summary>
        public DateTime DateOfPublication { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// 1: Hoạt động 0: Không hoạt động
        /// </summary>
        public bool? IsAction { get; set; }
        /// <summary>
        /// 1: Đã xóa; 0: Chưa xóa
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
    }
}
