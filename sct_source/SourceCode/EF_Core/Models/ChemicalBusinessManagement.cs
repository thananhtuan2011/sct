using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ChemicalBusinessManagement
    {
        /// <summary>
        /// Quản lý doanh nghiệp hoá chất
        /// </summary>
        public Guid ChemicalBusinessManagementId { get; set; }
        /// <summary>
        /// Tên doanh nghiệp
        /// </summary>
        public Guid BusinessName { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = null!;
        public Guid DistrictId { get; set; }
        public Guid CommuneId { get; set; }
        /// <summary>
        /// Tồn trữ hoá chất
        /// </summary>
        public string ChemicalStorage { get; set; } = null!;
        public string Represent { get; set; } = null!;
        /// <summary>
        /// Xây dựng biện pháp PNUPSCHC 
        /// </summary>
        public bool Pnupschcmeasures { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Status { get; set; }
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
