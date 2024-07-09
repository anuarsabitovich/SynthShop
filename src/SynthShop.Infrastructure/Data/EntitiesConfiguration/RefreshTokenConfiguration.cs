using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.EntitiesConfiguration;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Token);
        builder.Property(x => x.Token).HasDefaultValueSql("NEWID()");
        builder.HasOne<User>(u => u.User)
            .WithMany(t => t.RefreshTokens)
            .HasForeignKey(x => x.UserId);
    }
}