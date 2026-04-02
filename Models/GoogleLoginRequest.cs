using System.ComponentModel.DataAnnotations;

namespace Propertia.Models
{
    public class GoogleLoginRequest
    {
        [Required]
        public string Credential { get; set; } = null!;
        public string? Role { get; set; }
    }
}
