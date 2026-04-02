namespace Propertia.Models
{
    public class Amenity
    {
        public int AmenityId { get; set; }
        public string AmenityName { get; set; } = null!;
        public ICollection<PropertyAmenity>? PropertyAmenities { get; set; } = new List<PropertyAmenity>();
    }
    public class AmenityDto
    {
        public int AmenityId { get; set; }

        public string AmenityName { get; set; }
    }

}
