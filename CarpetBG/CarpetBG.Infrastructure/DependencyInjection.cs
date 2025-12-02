using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.DTOs.Products;
using CarpetBG.Application.DTOs.Users;
using CarpetBG.Application.Factories;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Application.Services;
using CarpetBG.Application.Validations;
using CarpetBG.Infrastructure.Data;
using CarpetBG.Infrastructure.Repositories;
using CarpetBG.Infrastructure.Seeders;
using CarpetBG.Infrastructure.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarpetBG.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IDateTimeProvider>(provider =>
        {

            var httpContext = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            var user = httpContext?.User;
            var timeZoneId = user?.FindFirst("https://yourdomain.com/timezone")?.Value ?? "Europe/Sofia";

            TimeZoneInfo tz;
            try
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Sofia");
            }

            return new DateTimeProvider(tz);
        });

        services.AddScoped<ISeeder, AdditionSeeder>();
        services.AddScoped<ISeederService, SeederService>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //Validators
        services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidator>();
        services.AddScoped<IValidator<OrderItemDto>, OrderItemDtoValidator>();
        services.AddScoped<IValidator<ProductDto>, ProductDtoValidator>();

        //Factories
        services.AddScoped<IAddressFactory, AddressFactory>();
        services.AddScoped<IOrderFactory, OrderFactory>();
        services.AddScoped<IUserFactory, UserFactory>();
        services.AddScoped<IOrderItemFactory, OrderItemFactory>();
        services.AddScoped<IProductFactory, ProductFactory>();

        // Services
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderItemService, OrderItemService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}

