using System.ComponentModel.DataAnnotations;

namespace Propertia.Models
{
    public class PropertyPrice
    {
        [Key]
        public int PropertyPriceId { get; set; }   // ✅ NEW PK

        public int PropertyId { get; set; }        // ✅ FK only
        public Property Property { get; set; } = null!;

        public int TransactionTypeId { get; set; }
        public TransactionType TransactionType { get; set; } = null!;

        public decimal Amount { get; set; }
    }

    public class PropertyPriceDto
    {
        public int PropertyId { get; set; } // or PropertyId if you prefer
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = null!;
    }

    public class PropertyPriceCreateDto
    {
        public int PropertyId { get; set; }
        public int TransactionTypeId { get; set; }
        public decimal Amount { get; set; }
    }
}