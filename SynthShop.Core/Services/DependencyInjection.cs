using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SynthShop.Core.Services.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthShop.Core.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<CategoryService>();
            services.AddScoped<CustomerService>();
            services.AddScoped<OrderItemService>();
            services.AddScoped<OrderService>();
            services.AddScoped<ProductService>();


            return services;
        }

    }
}
