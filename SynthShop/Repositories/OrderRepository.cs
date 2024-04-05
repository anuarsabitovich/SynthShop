using Microsoft.EntityFrameworkCore;
using SynthShop.Data;
using SynthShop.Data.Entities;

namespace SynthShop.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MainDbContext dbContext;

        public OrderRepository(MainDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            await dbContext.AddAsync(order);
            await dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> DeleteAsync(int id)
        {
            var existingOrder = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (existingOrder == null)
            {
                return null;
            }
            dbContext.Orders.Remove(existingOrder);
            await dbContext.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await dbContext.Orders.ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Order?> UpdateAsync(int id, Order order)
        {
            var existingOrder = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);

            if (existingOrder == null)
            {
                return null;
            }
            existingOrder.CustomerName = order.CustomerName;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.TotalAmount = order.TotalAmount;

            await dbContext.SaveChangesAsync();
            return existingOrder;
        }


    }
}
