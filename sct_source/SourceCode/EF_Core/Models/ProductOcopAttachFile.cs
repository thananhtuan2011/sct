using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class ProductOcopAttachFile
    {
        public Guid ProductOcopattachFileId { get; set; }
        public Guid ProductOcopid { get; set; }
        public string LinkFile { get; set; } = null!;
        /// <summary>
        /// 0: hình ảnh 1: file quyết định phê duyệt
        /// </summary>
        public int Type { get; set; }
    }
}
