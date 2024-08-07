﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.EntitiesConfiguration;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.OrderItemID);

        builder.Property(oi => oi.Quantity)
            .IsRequired();


        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderID);

        builder.HasOne(oi => oi.Product)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.ProductID);

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("getdate()");

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}