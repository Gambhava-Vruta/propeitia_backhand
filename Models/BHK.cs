namespace Propertia.Models
{
    public class BHK
    {
        public int BHKId { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
        public string BHKType { get; set; } = null!;       // 1 BHK, 2 BHK, 3 BHK
        public int TotalWashrooms { get; set; }
    }
    public class BHKdto
    {
        public int BHKId { get; set; }
        public int PropertyId { get; set; }
        public string BHKType { get; set; } = null!;       // 1 BHK, 2 BHK, 3 BHK
        public int TotalWashrooms { get; set; }
    }
}
