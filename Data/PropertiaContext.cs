using Microsoft.EntityFrameworkCore;
using Propertia.Models; 

public class PropertiaContext : DbContext
{
    public PropertiaContext(DbContextOptions<PropertiaContext> options)
            : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<PropertyAddress> PropertyAddresses { get; set; }
    public DbSet<TransactionType> TransactionTypes { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyPrice> PropertyPrices { get; set; }
    public DbSet<PropertyImage> PropertyImages { get; set; }
    public DbSet<PropertyInquiry> PropertyInquiries { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
    public DbSet<BHK> BHKs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PropertyAmenity composite primary key
        modelBuilder.Entity<PropertyAmenity>()
            .HasKey(pa => new { pa.PropertyId, pa.AmenityId });

        // Property - BHK one-to-one
        modelBuilder.Entity<Property>()
            .HasOne(p => p.BHK)
            .WithOne(b => b.Property)
            .HasForeignKey<BHK>(b => b.PropertyId);

        modelBuilder.Entity<PropertyPrice>()
            .HasOne(pp => pp.Property)
            .WithMany(p => p.PropertyPrices)
            .HasForeignKey(pp => pp.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PropertyPrice>()
            .HasOne(pp => pp.TransactionType)
            .WithMany(tt => tt.PropertyPrices)
            .HasForeignKey(pp => pp.TransactionTypeId);

        // Prevent duplicate Rent/Sale per property
        modelBuilder.Entity<PropertyPrice>()
            .HasIndex(pp => new { pp.PropertyId, pp.TransactionTypeId })
            .IsUnique();



        // Configure decimal precision for prices and area
        //modelBuilder.Entity<Property>()
        //    .Property(p => p.Price)
        //    .HasColumnType("decimal(12,2)");

        //modelBuilder.Entity<Property>()
        //    .Property(p => p.RentPrice)
        //    .HasColumnType("decimal(12,2)");

        modelBuilder.Entity<Property>()
            .Property(p => p.AreaSqft)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<PropertyPrice>()
            .Property(pp => pp.Amount)
            .HasColumnType("decimal(12,2)");

        // Optional: enforce string length, defaults, etc.
        modelBuilder.Entity<User>()
            .Property(u => u.UserType)
            .HasDefaultValue("buyer");

        modelBuilder.Entity<Property>()
            .Property(p => p.RequireType)
            .HasDefaultValue("any");

        modelBuilder.Entity<Property>()
            .Property(p => p.Status)
            .HasDefaultValue("ongoing");

        // Unique constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.CategoryName)
            .IsUnique();

        modelBuilder.Entity<Amenity>()
            .HasIndex(a => a.AmenityName)
            .IsUnique();

        // ✅ Property ↔ PropertyAddress (One Address → Many Properties)
        modelBuilder.Entity<Property>()
            .HasOne(p => p.PropertyAddress)
            .WithMany(pa => pa.Properties)
            .HasForeignKey(p => p.PropertyAddressId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PropertyInquiry>()
        .HasOne(pi => pi.Property)
        .WithMany(p => p.PropertyInquiries)
        .HasForeignKey(pi => pi.PropertyId)
        .OnDelete(DeleteBehavior.Cascade); // keep cascade here

        modelBuilder.Entity<PropertyInquiry>()
            .HasOne(pi => pi.User)
            .WithMany(u => u.PropertyInquiries)
            .HasForeignKey(pi => pi.UserId)
            .OnDelete(DeleteBehavior.Restrict); // prevent cascade here

    }
}
