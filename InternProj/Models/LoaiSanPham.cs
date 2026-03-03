namespace InternProj.Models
{
    public class LoaiSanPham
    {
        public int Id { get; set; }
        public string Ma_LSP { get; set; } = string.Empty;

        public string Ten_LSP { get; set; } = string.Empty;

        public string Ghi_Chu { get; set; } = string.Empty;

        public override string ToString() => $"{Ma_LSP}";
    }
}
