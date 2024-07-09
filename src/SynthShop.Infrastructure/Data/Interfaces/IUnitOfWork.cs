using System.Data;

namespace SynthShop.Infrastructure.Data.Interfaces;

public interface IUnitOfWork
{
    public Task SaveChangesAsync();
    public IDbTransaction BeginTransaction();
}