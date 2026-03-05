using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class PhieuXuatKhoData : PhieuXuatKhoRawData
    {
        public string SoPhieu { get; set; } = string.Empty;

        public DateTime NgayXuatKho { get; set; }

        public string TenSP { get ; set; } = string.Empty;
                    
        public string MaSP { get ; set; } = string.Empty;

        public string TenDonViTinh { get; set; } = string.Empty;

        public float ThanhTien { get; set; }
        public override string ToString() => $"{SoPhieu}";
    }
}
