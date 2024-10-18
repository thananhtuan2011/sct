using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class MultiLevelSalesParticipant
    {
        /// <summary>
        /// Quản lý người tham gia bán hàng đa cấp
        /// </summary>
        public Guid MultiLevelSalesParticipantsId { get; set; }
        /// <summary>
        /// Mã số
        /// </summary>
        public string MultiLevelSalesParticipantsCode { get; set; } = null!;
        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string ParticipantsName { get; set; } = null!;
        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; } = null!;
        /// <summary>
        /// Số CMND / CCCD
        /// </summary>
        public string IdentityCardNumber { get; set; } = null!;
        /// <summary>
        /// Ngày cấp CMND / CCCD
        /// </summary>
        public DateTime DateOfIssuance { get; set; }
        /// <summary>
        /// Nơi cấp CMND / CCCD
        /// </summary>
        public string PlaceOfIssue { get; set; } = null!;
        /// <summary>
        /// Giới tính: 1 - Nam, 2 - Nữ.
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// Ngày tham gia
        /// </summary>
        public DateTime JoinDate { get; set; }
        public string Province { get; set; } = null!;
        /// <summary>
        /// Địa chỉ cư trú
        /// </summary>
        public string Address { get; set; } = null!;
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
