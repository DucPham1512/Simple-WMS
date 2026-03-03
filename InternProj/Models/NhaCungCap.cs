using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class NhaCungCap
    {
        public int Id { get; set; }
        public string Ma_Ncc { get; set; } = string.Empty;

        public string Ten_Ncc { get; set; } = string.Empty;
        public string Ghi_Chu { get; set; } = string.Empty;

        public override string ToString() => $"{Ma_Ncc}";
    }
}
