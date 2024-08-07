﻿using SynthShop.Domain.Entities;

namespace SynthShop.DTO;

public class BasketDTO
{
    public Guid BasketId { get; set; }
    public Guid? CustomerId { get; set; }
    public List<BasketItem>? Items { get; set; }
}