namespace InternProj.Models
{
    public class KhoUser
    {
        public int Id { get; set; }
        public string MaDangNhap { get; set; } = string.Empty;

        public int KhoId { get; set; }

        public override string ToString() => $"{Id}";
    }
}
