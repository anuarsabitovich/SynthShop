using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SynthShop.Core.Services.Impl;
using SynthShop.Core.Services.Interfaces;
using Amazon.S3;
using SynthShop.Domain.Settings;
using Amazon.SimpleEmail;

namespace SynthShop.Core.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderItemService, OrderItemService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddAWSServices(configuration);
        services.Configure<RabbitMQSettings>(options => configuration.GetSection("RabbitMQ").Bind(options));
        services.AddSingleton<EmailProducer>();
        services.AddHostedService<EmailConsumerService>();
        services.AddSingleton<EmailService>();
        return services;
    }

    public static IServiceCollection AddAWSServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AWSSettings>(options => configuration.GetSection("AWS").Bind(options));
        services.AddAWSService<IAmazonS3>();
        services.AddScoped<IStorageService, StorageService>();
        services.AddAWSService<IAmazonSimpleEmailService>();

        return services;
    }
}