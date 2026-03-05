using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class NhapKhoReportData
    {

        public DateTime NgayNhap { get; set; }
        public string SoPhieu { get; set; } = string.Empty;

        public string TenNCC { get; set; } = string.Empty;

        public string MaSP { get ; set; } = string.Empty;

        public string TenSP { get ; set; } = string.Empty;
                    
        public int SoLuong { get; set; }

        public float DonGia { get; set; }

        public float ThanhTien { get; set; }
        public override string ToString() => $"{SoPhieu}";
    }
}
