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
using CarpetBG.Infrastructure.Options;
using CarpetBG.Infrastructure.Repositories;
using CarpetBG.Infrastructure.Seeders;
using CarpetBG.Infrastructure.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CarpetBG.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        // DateTimeProvider scoped per request
        services.AddSingleton<IDateTimeProvider>(provider =>
        {
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor?.HttpContext;

            var defaultTimeZoneId = "Europe/Sofia"; // default
            var timeZoneId = string.Empty;
            if (httpContext?.User != null)
            {
                timeZoneId = httpContext.User
                    .FindFirst("https://yourdomain.com/timezone")?.Value ?? defaultTimeZoneId;
            }

            TimeZoneInfo tz;
            try
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById(defaultTimeZoneId);
            }

            return new DateTimeProvider(tz);
        });

        // Seeders
        services.AddScoped<ISeeder, RoleSeeder>();
        services.AddScoped<ISeeder, ProductSeeder>();
        services.AddScoped<ISeeder, UserSeeder>();
        services.AddScoped<ISeederService, SeederService>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Validators
        services.AddScoped<IValidator<CreateCustomerDto>, CreateCustomerDtoValidator>();
        services.AddScoped<IValidator<OrderItemDto>, OrderItemDtoValidator>();
        services.AddScoped<IValidator<ProductDto>, ProductDtoValidator>();

        // Factories
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
        services.AddScoped<IUserService, UserService>();

        // Document generation
        services.AddScoped<IOrderDocumentService, OrderDocumentService>();
        services.AddScoped<IOrderPdfGenerator, OrderPdfGenerator>();

        // Memory cache
        services.AddMemoryCache();

        return services;
    }

    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind & validate Postgre options
        services.AddOptions<PostgreOptions>()
            .Bind(configuration.GetSection(PostgreOptions.SectionName))
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.DbName) &&
                !string.IsNullOrWhiteSpace(o.DefaultConnection),
                "Postgre configuration is invalid")
            .ValidateOnStart();

        services.AddOptions<SuperAdminOptions>()
            .Bind(configuration.GetSection(SuperAdminOptions.SectionName))
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.Email),
                "SuperAdmin configuration is invalid")
            .ValidateOnStart();

        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.SectionName))
            .Validate(o =>
                !string.IsNullOrWhiteSpace(o.Authority) &&
                !string.IsNullOrWhiteSpace(o.Audience),
                "Auth configuration is invalid")
            .ValidateOnStart();

        // DbContext with pooling, retry, migrations
        services.AddDbContextPool<AppDbContext>((sp, options) =>
        {
            var pg = sp.GetRequiredService<IOptions<PostgreOptions>>().Value;

            options.UseNpgsql(pg.DefaultConnection, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                npgsql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });
        });

        // Register abstraction for Application layer
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
