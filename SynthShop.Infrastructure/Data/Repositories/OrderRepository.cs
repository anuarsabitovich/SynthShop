using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Enums;
using SynthShop.Infrastructure.Data;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MainDbContext _dbContext;

        public OrderRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }




        public async Task<Order> CreateOrder(Guid basketId, Guid customerId)
        {
            int retryCount = 0;
            const int maxRetries = 3;

            while (retryCount < maxRetries)
            {
                try
                {
                    var basket = await _dbContext.Baskets
                        .Include(b => b.Items)
                        .ThenInclude(i => i.Product)
                        .FirstOrDefaultAsync(b => b.BasketId == basketId);

                    if (basket == null)
                        throw new InvalidOperationException("Basket not found.");

                    if (!basket.CustomerId.HasValue)
                    {
                        basket.CustomerId = customerId;
                    }

                    var availabilityIssues = new List<string>();
                    foreach (var item in basket.Items)
                    {
                        if (item.Product.StockQuantity < item.Quantity)
                        {
                            availabilityIssues.Add($"Not enough stock for {item.Product.Name}. Requested: {item.Quantity}, Available: {item.Product.StockQuantity}");
                        }
                        else
                        {
                            item.Product.StockQuantity -= item.Quantity;
                        }
                    }

                    if (availabilityIssues.Any())
                    {
                        throw new InvalidOperationException("There are issues with product availability: " + string.Join(", ", availabilityIssues));
                    }

                    var order = new Order
                    {
                        OrderID = Guid.NewGuid(),
                        OrderDate = DateTime.UtcNow,
                        CustomerID = basket.CustomerId.Value,
                        Status = OrderStatus.Pending,
                        OrderItems = basket.Items.Select(bi => new OrderItem
                        {
                            OrderItemID = Guid.NewGuid(),
                            ProductID = bi.ProductId,
                            Quantity = bi.Quantity,
                            Price = bi.Product.Price,
                            CreatedAt = DateTime.UtcNow
                        }).ToList(),
                        TotalAmount = basket.Items.Sum(i => i.Product.Price * i.Quantity),
                        CreatedAt = DateTime.UtcNow
                    };

                    _dbContext.Orders.Add(order);
                    await _dbContext.SaveChangesAsync();

                    return order; 
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                    if (retryCount == maxRetries - 1)
                    {
                        throw; 
                    }
                    retryCount++; 
                }
            }

            throw new InvalidOperationException("Failed to create order due to concurrency conflicts.");
        }








        public async Task CancelOrder(Guid orderId)
        {
            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .SingleOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            if (order.Status == OrderStatus.Completed)
            {
                throw new InvalidOperationException("Completed orders cannot be cancelled.");
            }


            if (order.Status == OrderStatus.Pending)
            {
                foreach (var item in order.OrderItems)
                {
                    var product = item.Product;  
                    product.StockQuantity += item.Quantity;
                }
            }
            
            order.Status = OrderStatus.Cancelled;


            await _dbContext.SaveChangesAsync();
        }


        public async Task CompleteOrder(Guid orderId)
        {
            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            if (order.Status == OrderStatus.Completed)
            {
                throw new InvalidOperationException("Order is already completed.");
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot complete a cancelled order.");
            }

            order.Status = OrderStatus.Completed;

            await _dbContext.SaveChangesAsync(); 
        }
    }
}
