﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.EntitiesConfiguration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.OrderID);

        builder.Property(o => o.OrderDate)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(o => o.UserId)
            .IsRequired();


        builder.HasOne(o => o.User)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.UserId)
            .IsRequired();

        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("getdate()");

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}