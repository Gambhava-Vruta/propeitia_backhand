using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Propertia.Models
{
    public class PropertiaContextFactory : IDesignTimeDbContextFactory<PropertiaContext>
    {
        public PropertiaContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PropertiaContext>();
            optionsBuilder.UseSqlServer(
                "Server=LAPTOP-7P28N3H4\\SQLEXPRESS;Database=Propertia_database;Trusted_Connection=True;TrustServerCertificate=True"
            );

            return new PropertiaContext(optionsBuilder.Options);
        }
    }
}
