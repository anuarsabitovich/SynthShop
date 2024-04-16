﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SynthShop.Infrastructure.Data.Repositories;
using SynthShop.Infrastructure.Domain.Intefaces;

namespace SynthShop.Infrastructure.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices( this IServiceCollection services, IConfiguration configuration)
        {

            // Dependency injection for Db Context
            services.AddDbContext<MainDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("MainDbConnectionString")));

            // Dependency injection for Repository
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            // Adding Auto Mapper

            return services;
        }
    }
}
