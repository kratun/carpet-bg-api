using CarpetBG.Application.DTOs.Customers;
using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.DTOs.Products;
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
            var loggedUser = httpContext?.User;

            // TODO handle timezone properly and fallback to env variable
            var timeZoneId = loggedUser?.FindFirst("https://yourdomain.com/timezone")?.Value ?? "Europe/Sofia";

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

        services.AddScoped<ISeeder, RoleSeeder>();
        services.AddScoped<ISeeder, ProductSeeder>();
        services.AddScoped<ISeederService, SeederService>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //Validators
        services.AddScoped<IValidator<CreateCustomerDto>, CreateCustomerDtoValidator>();
        services.AddScoped<IValidator<OrderItemDto>, OrderItemDtoValidator>();
        services.AddScoped<IValidator<ProductDto>, ProductDtoValidator>();

        //Factories
        services.AddScoped<IAddressFactory, AddressFactory>();
        services.AddScoped<IOrderFactory, OrderFactory>();
        services.AddScoped<ICustomerFactory, CustomerFactory>();
        services.AddScoped<IOrderItemFactory, OrderItemFactory>();
        services.AddScoped<IProductFactory, ProductFactory>();

        // Services
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderItemService, OrderItemService>();
        services.AddScoped<IProductService, ProductService>();

        // Memory cache
        services.AddMemoryCache();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
        }));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}

