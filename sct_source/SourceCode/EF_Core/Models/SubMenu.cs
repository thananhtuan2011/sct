using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class SubMenu
    {
        public int IdSub { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? Link { get; set; }
        public string? PageKey { get; set; }
        public int? Position { get; set; }
        public string? Icon { get; set; }
        public string? Target { get; set; }
        public string? IdMainMenu { get; set; }
        public string? AllowPermit { get; set; }
    }
}
