using System.ComponentModel.DataAnnotations;

namespace Propertia.Models
{
    public class PropertyAddress
    {
        [Key]
        public int PropertyAddressId { get; set; }

        public string? Location { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? SocietyName { get; set; }
        public string? Landmark { get; set; }
        public string? FamousArea { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
    public class PropertyAddressdto
    {
  
        public int PropertyAddressId { get; set; }

        public string? Location { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? SocietyName { get; set; }
        public string? Landmark { get; set; }
        public string? FamousArea { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    

    }
}
