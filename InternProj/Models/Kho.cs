using System;
using System.Collections.Generic;
using System.Text;

namespace InternProj.Models
{
    public class Kho
    {
        public int Id { get; set; }
        public string Ten_Kho { get; set; } = string.Empty;
        public string Ghi_Chu { get; set; } = string.Empty;

        public override string ToString() => $"{Ten_Kho}";
    }
}
