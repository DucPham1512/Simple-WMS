using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class SanPham
    {
        public int Id { get; set; }
        public string Ma_SP { get; set; } = string.Empty;

        public string Ten_SP { get; set; } = string.Empty;

        public int Id_LSP { get; set; }

        public int Id_DVT { get; set; }
        public string Ghi_Chu { get; set; } = string.Empty;

        public override string ToString() => $"{Ma_SP}";
    }
}
