using Microsoft.Extensions.Logging;

namespace CarpetBG.Infrastructure.Seeders;

public class SeederService(IEnumerable<ISeeder> seeders, ILogger<SeederService> logger) : ISeederService
{
    public async Task SeedAllAsync()
    {
        logger.LogInformation("Starting database seeding...");

        foreach (var seeder in seeders)
        {
            logger.LogInformation("Running seeder: {Seeder}", seeder.GetType().Name);
            await seeder.SeedAsync();
        }

        logger.LogInformation("Seeding completed.");
    }
}
