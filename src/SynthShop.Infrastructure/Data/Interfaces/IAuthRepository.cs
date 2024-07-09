using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces;

public interface IAuthRepository
{
    Task<RefreshToken?> GetRefreshTokenById(Guid token);
    Task UpdateRefreshToken(RefreshToken token);
    Task AddRefreshToken(RefreshToken token);
}