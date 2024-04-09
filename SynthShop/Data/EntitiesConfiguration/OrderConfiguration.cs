using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynthShop.Data.Entities;

namespace SynthShop.Data.EntitiesConfiguration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.OrderID);

            builder.Property(o => o.OrderDate)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(o => o.CustomerID)
                .IsRequired();

            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            builder.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerID)
                .IsRequired();

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderID);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasQueryFilter(c => !c.IsDeleted);

        }

    }
}
