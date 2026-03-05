using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class PhieuXuatKhoRawData
    {
        public int Id { get; set; }

        public int XuatKhoId { get; set; }

        public int SanPhamId { get; set; }

        public int SoLuong { get; set; }

        public float DonGia { get; set; }

        public override string ToString() => $"{XuatKhoId}";
    }
}
