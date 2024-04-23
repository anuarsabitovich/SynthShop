using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthShop.Core.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrder(Guid basketId, Guid customerId);
        
        Task CancelOrder(Guid orderID);

        Task CompleteOrder(Guid basketId);
    }
}
