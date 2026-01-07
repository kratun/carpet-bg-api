using System.Text.Json;
using System.Text.Json.Serialization;

using CarpetBG.API.Extensions;
using CarpetBG.API.Middleware;
using CarpetBG.Infrastructure;
using CarpetBG.Infrastructure.Data;
using CarpetBG.Infrastructure.Seeders;
using CarpetBG.Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddPersistence(builder.Configuration)
    .AddRepositories();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicies.Default, policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure JWT authentication for local testing with fake token
builder.Services.AddAuthConfiguration(builder.Configuration);


builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors(CorsPolicies.Default);

app.UseMiddleware<ResultMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    using var scope = app.Services.CreateScope();

    //var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //await db.Database.MigrateAsync();

    DBInitializer.MigrateDatabase(app.Services);

    var seeder = scope.ServiceProvider.GetRequiredService<ISeederService>();
    await seeder.SeedAllAsync();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
