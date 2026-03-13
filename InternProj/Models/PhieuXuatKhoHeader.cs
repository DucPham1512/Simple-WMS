using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class PhieuXuatKhoHeader
    {
        public int Id { get; set; }
        public string So_Phieu_Xuat_Kho { get; set; } = string.Empty;

        public int Kho_ID { get; set; }

        public string Ten_Kho { get; set; } = string.Empty;

        public DateTime Ngay_Xuat_Kho { get; set; }

        public string Ghi_Chu { get; set; } = string.Empty;
        public override string ToString() => $"{So_Phieu_Xuat_Kho}";
    }
}	

