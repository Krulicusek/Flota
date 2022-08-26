using Microsoft.EntityFrameworkCore;

namespace Flota.Server.Entity
{
    public class FlotaDBContext : DbContext
    {
        public FlotaDBContext(DbContextOptions<FlotaDBContext> options) : base(options) { }

        public DbSet<ProductType> ProductTypes { get; set; }

    }
}
