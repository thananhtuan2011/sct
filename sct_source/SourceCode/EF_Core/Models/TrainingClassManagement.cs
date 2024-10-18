﻿using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TrainingClassManagement
    {
        public Guid TrainingClassManagementId { get; set; }
        public string Topic { get; set; } = null!;
        public DateTime TimeStart { get; set; }
        public string Location { get; set; } = null!;
        public string Participant { get; set; } = null!;
        public int NumberOfAttendees { get; set; }
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