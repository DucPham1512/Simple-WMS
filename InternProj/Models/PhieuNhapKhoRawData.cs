using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class PhieuNhapKhoRawData
    {
        public int Id { get; set; }

        public int NhapKhoId { get; set; }

        public int SanPhamId { get; set; }

        public decimal SoLuong { get; set; }

        public decimal DonGia { get; set; }

        public override string ToString() => $"{NhapKhoId}";
    }
}
