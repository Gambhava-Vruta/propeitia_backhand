using System.ComponentModel.DataAnnotations;

namespace Propertia.Models
{

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Phone { get; set; }
        public string UserType { get; set; } = "buyer";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<PropertyInquiry> PropertyInquiries { get; set; } = new List<PropertyInquiry>();
    }
    public class Userdto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Password { get; set; } = null!;
        public string? Phone { get; set; }
        public string UserType { get; set; } = "buyer";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class UpdateUserDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        public string UserType { get; set; }
    }


}