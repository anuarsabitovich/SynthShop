using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
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



        public async Task<Order?> GetOrderAsync(Guid orderId)
        {
            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .SingleOrDefaultAsync(o => o.OrderID == orderId);
            
            return order;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _dbContext.Orders.Add(order);
            return order;
        }

        public async Task DeleteOrderAsync(Guid orderId)
        {
            var order = new Order() { OrderID = orderId, IsDeleted = true };
            _dbContext.Orders.Attach(order);
            _dbContext.Entry(order).Property(e => e.IsDeleted).IsModified = true;
        }

        public async Task<Order?> UpdateOrderAsync(Guid orderId, Order order)
        {
            

            var existingOrder = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == orderId);
            if (existingOrder == null)
            {
                return null;
            }
     
            _dbContext.Entry(existingOrder).CurrentValues.SetValues(order);

            return order;
        }
        public async Task<List<Order>> GetOrdersByCustomerId(Guid customerId)
        {
            return await _dbContext.Orders
                .Where(o => o.UserId == customerId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
