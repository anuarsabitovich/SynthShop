using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.EntitiesConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {

            builder.Property(c => c.FirstName)
                .HasMaxLength(50);

            builder.Property(c => c.LastName)
                .HasMaxLength(50);

            builder.Property(c => c.Email)
                .HasMaxLength(100);

            builder.Property(c => c.Address)
                .HasMaxLength(200);

            builder.HasMany(c => c.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("getdate()");

            builder.ToTable("User");



        }
    }
}
