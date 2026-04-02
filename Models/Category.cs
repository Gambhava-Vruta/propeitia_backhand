namespace Propertia.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
    public class Categorydto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

    }
}
