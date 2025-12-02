
using CarpetBG.Domain.Entities;
using CarpetBG.Domain.Enums;
using CarpetBG.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarpetBG.Infrastructure.Seeders;

public class AdditionSeeder(AppDbContext context, ILogger<AdditionSeeder> logger) : ISeeder
{
    public async Task SeedAsync()
    {
        foreach (var item in SeedData)
        {
            var exists = await context.Additions.AnyAsync(i => i.NormalizedName == item.NormalizedName);
            if (!exists)
            {
                context.Additions.Add(item);
                logger.LogInformation($"Seeded Addtion: {item.NormalizedName}");
            }
            else
            {
                logger.LogInformation($"Addition already exists: {item.NormalizedName}");
            }
        }

        await context.SaveChangesAsync();
    }

    private static List<Addition> SeedData => [
        new()
        {
            Id = Guid.NewGuid(),
            AdditionType = AdditionTypes.AppliedAsPercentage,
            Name = "Express oreder",
            NormalizedName = "EXPRESS ORDER",
            Value = 1.5m
        }
    ];
}
