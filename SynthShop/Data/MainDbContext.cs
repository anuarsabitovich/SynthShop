using Microsoft.EntityFrameworkCore;
using SynthShop.Data.Entities;

namespace SynthShop.Data
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
