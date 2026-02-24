using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class LoaiSanPham
    {
        public int Ma_LSP { get; set; }

        public string Ten_LSP { get; set; } = string.Empty;

        public string Ghi_Chu { get; set; } = string.Empty;

        public override string ToString() => $"{Ma_LSP}";
    }
}
