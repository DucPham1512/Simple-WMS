using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class XuatNhapTonData
    {
        public string MaSP { get; set; } = string.Empty;

        public string TenSP { get; set; } = string.Empty;

        public decimal SoLuongDauKy { get; set; }

        public decimal SoLuongNhap { get; set; }

        public decimal SoLuongXuat { get; set; }

        public decimal SoLuongCuoiKy { get; set; }
        public override string ToString() => $"{MaSP}";
    }
}	

