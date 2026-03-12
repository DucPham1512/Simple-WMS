using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class XuatKhoReportData
    {

        public DateTime NgayXuat { get; set; }
        public string SoPhieu { get; set; } = string.Empty;

        public string MaSP { get ; set; } = string.Empty;

        public string TenSP { get ; set; } = string.Empty;
                    
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public decimal ThanhTien { get; set; }
        public override string ToString() => $"{SoPhieu}";
    }
}
