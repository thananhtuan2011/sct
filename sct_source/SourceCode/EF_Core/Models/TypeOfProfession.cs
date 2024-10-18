﻿using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class TypeOfProfession
    {
        public Guid TypeOfProfessionId { get; set; }
        public string TypeOfProfessionCode { get; set; } = null!;
        public string TypeOfProfessionName { get; set; } = null!;
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
