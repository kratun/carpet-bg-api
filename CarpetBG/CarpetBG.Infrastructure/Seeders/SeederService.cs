using Microsoft.Extensions.Logging;

namespace CarpetBG.Infrastructure.Seeders;

public class SeederService(ISeeder additionSeeder, ILogger<SeederService> logger) : ISeederService
{
    public async Task SeedAllAsync()
    {
        logger.LogInformation("Starting database seeding...");

        await additionSeeder.SeedAsync();

        logger.LogInformation("Seeding completed.");
    }
}
