using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface IBasketRepository
    {
        Task<Guid> CreateBasketAsync();
        Task<Basket?> GetBasketByIdAsync(Guid basketId);

        Task DeleteBasketAsync(Guid basketId);


    }
}
