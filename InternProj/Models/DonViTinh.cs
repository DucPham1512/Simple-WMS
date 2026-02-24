using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class DonViTinh
    {
        public string Ten_Don_Vi_Tinh { get; set; } = string.Empty;
        public string Ghi_Chu { get; set; } = string.Empty;

        public override string ToString() => $"{Ten_Don_Vi_Tinh}";
    }
}
