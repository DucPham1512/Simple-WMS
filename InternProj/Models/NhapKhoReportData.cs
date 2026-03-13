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
                    
        public decimal SoLuong { get; set; }

        public decimal DonGia { get; set; }

        public decimal ThanhTien { get; set; }
        public override string ToString() => $"{SoPhieu}";
    }
}
