using CarpetBG.Infrastructure.Seeders;

namespace CarpetBG.API.Extensions;

public static class HostExtensions
{
    public static async Task SeedDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var seeder = services.GetRequiredService<ISeederService>();
            await seeder.SeedAllAsync();
            logger.LogInformation("Database seeding finished.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database seeding.");
            throw;
        }
    }
}
