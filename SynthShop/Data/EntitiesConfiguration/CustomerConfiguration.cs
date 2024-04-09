using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynthShop.Data.Entities;

namespace SynthShop.Data.EntitiesConfiguration
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.CustomerID);

            builder.Property(c => c.FirstName)
                .HasMaxLength(50);

            builder.Property(c => c.LastName)
                .HasMaxLength(50);

            builder.Property(c => c.Email)
                .HasMaxLength(100);

            builder.Property(c => c.Address)
                .HasMaxLength(200);

            builder.HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerID);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasQueryFilter(c => !c.IsDeleted);

        }
    }
}
