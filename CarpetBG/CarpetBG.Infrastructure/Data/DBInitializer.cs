using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CarpetBG.Infrastructure.Data;

public static class DBInitializer
{
    /// <summary>
    /// Ensures the database exists and applies any pending migrations.
    /// Safe to call at startup. Use caution in production.
    /// </summary>
    public static void MigrateDatabase(IServiceProvider serviceProvider, bool applyMigrations = true)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            //// Create database if it doesn't exist
            //dbContext.Database.EnsureCreated(); // Only creates DB if missing, skips if exists

            if (applyMigrations)
            {
                var pendingMigrations = dbContext.Database.GetPendingMigrations();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                    dbContext.Database.Migrate();
                    logger.LogInformation("Migrations applied successfully.");
                }
                else
                {
                    logger.LogInformation("No pending migrations.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error migrating database");
            throw;
        }
    }
}
