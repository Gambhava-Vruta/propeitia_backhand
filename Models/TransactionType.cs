namespace Propertia.Models
{
    public class TransactionType
    {
        public int TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; } = null!;

        public ICollection<PropertyPrice> PropertyPrices { get; set; } = new List<PropertyPrice>();
    }
    public class TransactionTypeDto
    {
        public int TransactionTypeId { get; set; }
        public string TransactionTypeName { get; set; } = null!;
    }
}