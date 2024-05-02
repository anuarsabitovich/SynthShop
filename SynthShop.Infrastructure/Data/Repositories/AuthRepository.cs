using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly MainDbContext _dbContext;

        public AuthRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RefreshToken?> GetRefreshTokenById(Guid token)
        {
            var storedRefreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == token);
            return storedRefreshToken;
        }

        public async Task UpdateRefreshToken(RefreshToken token)
        {
            _dbContext.RefreshTokens.Update(token);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken?> AddRefreshToken(RefreshToken token)
        {
            await _dbContext.RefreshTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();
            return token;
        }
    }
}
