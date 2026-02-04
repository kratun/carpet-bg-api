using CarpetBG.Infrastructure.Seeders;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Npgsql;

namespace CarpetBG.Infrastructure.Data;

public static class DBInitializer
{
    /// <summary>
    /// Ensures the database exists and applies any pending migrations.
    /// Safe to call at startup. Use caution in production.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider rootProvider, bool applyMigrations = true)
    {
        // Create a scope for all scoped services
        using var scope = rootProvider.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<AppDbContext>>();

        var options = services.GetRequiredService<IOptions<PostgreOptions>>().Value;
        await CreateDatabaseIfNotExistsAsync(options, logger);

        try
        {
            if (applyMigrations)
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                    await dbContext.Database.MigrateAsync();
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

        var seeder = services.GetRequiredService<ISeederService>();
        await seeder.SeedAllAsync();
    }

    public static async Task CreateDatabaseIfNotExistsAsync(PostgreOptions options, ILogger logger)
    {
        var builder = new NpgsqlConnectionStringBuilder(options.DefaultConnection)
        {
            Database = "postgres"
        };

        await using var connection = new NpgsqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        const string existsSql =
            """SELECT 1 FROM pg_database WHERE datname = @dbName""";

        await using var existsCmd = new NpgsqlCommand(existsSql, connection);
        existsCmd.Parameters.AddWithValue("dbName", options.DbName);

        var exists = await existsCmd.ExecuteScalarAsync();
        if (exists is not null) return;

        await using var createCmd =
            new NpgsqlCommand($"CREATE DATABASE \"{options.DbName}\"", connection);

        await createCmd.ExecuteNonQueryAsync();
        logger.LogInformation("Database {DbName} created.", options.DbName);
    }
}
