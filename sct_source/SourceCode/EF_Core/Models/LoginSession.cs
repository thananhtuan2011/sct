using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class LoginSession
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime LoginDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Locked { get; set; }
    }
}
