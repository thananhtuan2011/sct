using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateManageAncolLocalBussinesDetail
    {
        public Guid CateManageAncolLocalBussinesDetailId { get; set; }
        public string Fullname { get; set; } = null!;
        /// <summary>
        /// 1: thành viên góp vốn
        /// 2: cổ đông
        /// </summary>
        public int Type { get; set; }
        public Guid CateManageAncolLocalBussinessId { get; set; }
    }
}
