using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class XuatNhapTonData
    {
        public string MaSP { get; set; } = string.Empty;

        public string TenSP { get; set; } = string.Empty;

        public int SoLuongDauKy { get; set; }

        public int SoLuongNhap { get; set; }

        public int SoLuongXuat { get; set; }

        public int SoLuongCuoiKy { get; set; }
        public override string ToString() => $"{MaSP}";
    }
}	

