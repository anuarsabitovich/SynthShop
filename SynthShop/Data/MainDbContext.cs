using Microsoft.EntityFrameworkCore;
using SynthShop.Data.Entities;
using SynthShop.Data.EntitiesConfiguration;
using System.Reflection;

namespace SynthShop.Data
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                // Check if the entity type has IsDeleted property
                var isDeletedProperty = entry.Metadata.FindProperty("IsDeleted");
                if (isDeletedProperty != null)
                {
                    if (entry.State == EntityState.Deleted)
                    {
                        // Instead of physically deleting, mark the entity as deleted
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["IsDeleted"] = true;
                    }
                }
            }
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
