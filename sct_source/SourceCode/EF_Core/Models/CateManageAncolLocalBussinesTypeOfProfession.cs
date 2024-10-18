using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateManageAncolLocalBussinesTypeOfProfession
    {
        public Guid CateManageAncolLocalBussinesTypeProfessionId { get; set; }
        public Guid TypeOfProfessionId { get; set; }
        public Guid CateManageAncolLocalBussinessId { get; set; }
    }
}
