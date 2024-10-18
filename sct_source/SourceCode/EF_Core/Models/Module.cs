using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class Module
    {
        public int IdModule { get; set; }
        public string? Title { get; set; }
        public string? Link { get; set; }
        public int? Position { get; set; }
        public string? Icon { get; set; }
        public string? Target { get; set; }
        public string? Summary { get; set; }
        public string? AllowPermit { get; set; }
    }
}
