namespace Propertia.Models
{
    public class PropertyAmenity
    {
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
        public int AmenityId { get; set; }
        public Amenity Amenity { get; set; } = null!;
    }
}
