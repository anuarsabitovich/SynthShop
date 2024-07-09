using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly MainDbContext _dbContext;

    public UnitOfWork(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public IDbTransaction BeginTransaction()
    {
        var transaction = _dbContext.Database.BeginTransaction();

        return transaction.GetDbTransaction();
    }
}