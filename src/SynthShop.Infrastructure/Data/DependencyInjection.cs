using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;
using SynthShop.Infrastructure.Data.Repositories;

namespace SynthShop.Infrastructure.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Dependency injection for Db Context
        services.AddDbContext<MainDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("MainDbConnectionString")));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequiredLength = 12;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<MainDbContext>();


        // Dependency injection for Repository
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IBasketItemRepository, BasketItemRepository>();

        return services;
    }
}