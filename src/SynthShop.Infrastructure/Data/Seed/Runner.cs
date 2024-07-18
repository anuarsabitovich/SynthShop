using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SynthShop.Infrastructure.Data.Seed
{
    public sealed class Runner
    {
        private readonly IEnumerable<ISeeder> _seeders;
        private readonly MainDbContext _dbContext;

        public Runner(IEnumerable<ISeeder> seeders, MainDbContext dbContext)
        {
            _seeders = seeders;
            _dbContext = dbContext;
        }

        public async Task SeedAsync()
        {
            await _dbContext.Database.MigrateAsync();
            foreach (var seeder in _seeders)
            {
                await seeder.SeedAsync();
            }
        }
    }
}
