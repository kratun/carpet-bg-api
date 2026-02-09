using System.Text.Json;
using System.Text.Json.Serialization;

using CarpetBG.API.Extensions;
using CarpetBG.API.Middleware;
using CarpetBG.Infrastructure;
using CarpetBG.Infrastructure.Authentication;
using CarpetBG.Infrastructure.Data;
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
    .AddInfrastructureServices()
    .AddPersistence(builder.Configuration)
    .AddRepositories()
    .AddApplicationServices()
    .AddAuthenticationInfrastructure(builder.Configuration);

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

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors(CorsPolicies.Default);

app.UseMiddleware<ResultMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    // create a proper scope
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        // 2️⃣ Ensure database exists
        await DBInitializer.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Database initialization failed.");
        throw;
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
