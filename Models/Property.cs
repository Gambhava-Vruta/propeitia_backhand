using System.ComponentModel.DataAnnotations;

namespace Propertia.Models
{
    public class Property
    {
        public int PropertyId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int PropertyAddressId { get; set; }
        public PropertyAddress PropertyAddress { get; set; } = null!;


        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? TermsAndConditions { get; set; }
        public string RequireType { get; set; } = "any";

        public decimal AreaSqft { get; set; }

        public string Status { get; set; } = "ongoing";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();
        public ICollection<PropertyInquiry> PropertyInquiries { get; set; } = new List<PropertyInquiry>();
        public ICollection<PropertyPrice> PropertyPrices { get; set; }
            = new List<PropertyPrice>();
        public ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
        public BHK BHK { get; set; } = null!;
    }
    public class PropertyDto
    {
        public int PropertyId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string RequireType { get; set; } = "any";
        public decimal AreaSqft { get; set; }
        public string Status { get; set; } = "ongoing";
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int PropertyAddressId { get; set; }
            public IEnumerable<string>? Images { get; set; }

        public IEnumerable<PropertyPriceDto>? PropertyPrices { get; set; }
        // Optionally, you can add simplified DTOs for BHK, Images, etc.
    }
    public class PropertyCreateDto
    {
        // Property info
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal AreaSqft { get; set; }
        public string Status { get; set; } = "ongoing";
        public string RequireType { get; set; } = "any"; // "rent", "sale", "any"
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int PropertyAddressId { get; set; }

        // BHK info
        public PropertyAddressdto Address { get; set; } = null!;

        public string BHKType { get; set; } = null!;
        public int TotalWashrooms { get; set; }

        // Optional PropertyPrices
        public decimal? SalePrice { get; set; }
        public decimal? RentPrice { get; set; }
        // Optional Images (allow many)
        public IFormFileCollection? Images { get; set; }

        //public List<string>? ImagePaths { get; set; }

        // Optional Amenities (allow many)
        public List<int>? AmenityIds { get; set; }

    }
    public class PropertyUpdateDto
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal AreaSqft { get; set; }

        [Required]
        public string Status { get; set; }

        public string RequireType { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int TransactionTypeId { get; set; }

        [Required]
        public PropertyAddressdto Address { get; set; } = null!;

        public string BHKType { get; set; }
        public int TotalWashrooms { get; set; }

        // ⚠️ IMPORTANT: No validation attributes on prices
        // Backend logic determines which prices are required based on TransactionTypeId
        public decimal? SalePrice { get; set; }
        public decimal? RentPrice { get; set; }

        public List<IFormFile>? Images { get; set; }
        public List<int>? AmenityIds { get; set; }
    }



    // Keep your existing PropertyAddressDto

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }  // ← IEnumerable instead of List<T>
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }

}