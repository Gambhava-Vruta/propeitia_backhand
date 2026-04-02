using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Propertia.Models
{
    public class PropertiaContextFactory : IDesignTimeDbContextFactory<PropertiaContext>
    {
        public PropertiaContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PropertiaContext>();
            // Use a dummy Postgres connection string for design-time (EF migrations CLI).
            // At runtime, the real connection string comes from environment variables on Render.
            optionsBuilder.UseNpgsql(
                "Host=localhost;Database=propertia_dev;Username=postgres;Password=postgres"
            );

            return new PropertiaContext(optionsBuilder.Options);
        }
    }
}
