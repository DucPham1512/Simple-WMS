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
                    
        public float SoLuong { get; set; }
        public float DonGia { get; set; }

        public float ThanhTien { get; set; }
        public override string ToString() => $"{SoPhieu}";
    }
}
