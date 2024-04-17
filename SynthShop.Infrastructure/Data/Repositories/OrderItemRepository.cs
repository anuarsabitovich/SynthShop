using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data;
using SynthShop.Infrastructure.Domain.Intefaces;

namespace SynthShop.Infrastructure.Data.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {

        private readonly MainDbContext _dbContext;

        public OrderItemRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OrderItem> CreateAsync(OrderItem orderItem)
        {
            await _dbContext.AddAsync(orderItem);
            await _dbContext.SaveChangesAsync();
            return orderItem;
        }

        public async Task<List<OrderItem>> GetAllAsync()
        {
            var categories = await _dbContext.OrderItems.AsNoTracking().ToListAsync();

            return categories;
        }

        public async Task<OrderItem?> GetByIdAsync(Guid id)
        {
            var existingOrderItem = await _dbContext.OrderItems.FirstOrDefaultAsync(x => x.OrderItemID == id);

            if (existingOrderItem == null)
            {
                return null;
            }
            return existingOrderItem;
        }

        public async Task<OrderItem?> UpdateAsync(Guid id, OrderItem orderItem)
        {
            var existingOrderItem = await _dbContext.OrderItems.FirstOrDefaultAsync(x => x.OrderItemID == id);

            if (existingOrderItem == null)
            {
                return null;
            }
            existingOrderItem.OrderID =orderItem.OrderID;
            existingOrderItem.ProductID = orderItem.ProductID;
            existingOrderItem.Quantity = orderItem.Quantity;
            existingOrderItem.Price = orderItem.Price;
            existingOrderItem.UpdateAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return orderItem;

        }
        public async Task<OrderItem?> DeleteAsync(Guid id)
        {
            var existingOrder = await _dbContext.OrderItems.FirstOrDefaultAsync(x => x.OrderItemID == id);
            if (existingOrder == null)
            {
                return null;
            }

            existingOrder.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return existingOrder;

        }
    }
}
