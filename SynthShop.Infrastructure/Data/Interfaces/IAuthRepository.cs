using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface IAuthRepository
    { 
        Task<RefreshToken?> GetRefreshTokenById(Guid token);
        Task UpdateRefreshToken(RefreshToken token);
        Task<RefreshToken?> AddRefreshToken(RefreshToken token);

    }
}
