﻿using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class PetroleumBusiness
    {
        public Guid PetroleumBusinessId { get; set; }
        public Guid PetroleumBusinessName { get; set; }
        public string Supplier { get; set; } = null!;
        public string? Address { get; set; }
        public string? Representative { get; set; }
        public string? PhoneNumber { get; set; }
        public bool JoinZaloOa { get; set; }
        public string? TimeRegister { get; set; }
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
