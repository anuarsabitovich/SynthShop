using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data;
using SynthShop.Infrastructure.Domain.Intefaces;

namespace SynthShop.Infrastructure.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MainDbContext _dbContext;

        public OrderRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Order> CreateAsync(Order order)
        {
            await _dbContext.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }
        public async Task<List<Order>> GetAllAsync()
        {
            var orders = _dbContext.Orders.AsNoTracking().ToListAsync();

            return await orders;  
        }
        public async Task<Order?> DeleteAsync(Guid id)
        {
            var existingOrder = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == id);
            if (existingOrder == null)
            {
                return null;
            }
            
            existingOrder.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return existingOrder;

        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            var existingOrder = _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == id);
            if (existingOrder == null)
            {
                return null;
            }
            return await existingOrder;

        }

   

        public async Task<Order?> UpdateAsync(Guid id, Order order)
        {
            var existingOrder = await _dbContext.Orders.FirstOrDefaultAsync(x => x.OrderID == id);
            if (existingOrder == null)
            {
                return null;
            }
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.CustomerID = order.CustomerID;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.Customer = order.Customer;
            existingOrder.OrderItems = order.OrderItems;
            existingOrder.UpdateAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return order;
        }
    }
}
