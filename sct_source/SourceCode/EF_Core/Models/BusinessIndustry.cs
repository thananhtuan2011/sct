using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class BusinessIndustry
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public Guid IndustryId { get; set; }
    }
}
