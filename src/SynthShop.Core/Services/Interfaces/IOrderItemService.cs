﻿using SynthShop.Domain.Entities;

namespace SynthShop.Core.Services.Interfaces;

public interface IOrderItemService
{
    Task CreateAsync(OrderItem orderItem);
    Task<List<OrderItem>> GetAllAsync();
    Task<OrderItem?> GetByIdAsync(Guid id);
    Task<OrderItem?> UpdateAsync(Guid id, OrderItem orderItem);
    Task<OrderItem?> DeleteAsync(Guid id);
}