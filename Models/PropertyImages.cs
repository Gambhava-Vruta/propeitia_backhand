using System.ComponentModel.DataAnnotations;

namespace Propertia.Models
{
    public class PropertyImage
    {
        [Key]
        public int ImageId { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
        public string? ImagePath { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
    public class PropertyImagedto
    {
        public int ImageId { get; set; }
        public int PropertyId { get; set; }
        public string? ImagePath { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}