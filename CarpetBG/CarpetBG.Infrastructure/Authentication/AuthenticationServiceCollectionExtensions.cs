using CarpetBG.Infrastructure.Authorization;
using CarpetBG.Infrastructure.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarpetBG.Infrastructure.Authentication;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind & validate Auth options
        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.SectionName))
            .ValidateOnStart();

        // JWT events handler
        services.AddScoped<JwtBearerEventsHandler>();

        // Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var auth = configuration
                    .GetSection(AuthOptions.SectionName)
                    .Get<AuthOptions>()!;

                options.Authority = auth.Authority;
                options.Audience = auth.Audience;
                options.TokenValidationParameters.ValidIssuer = auth.Authority;
                options.EventsType = typeof(JwtBearerEventsHandler);
            });

        // Authorization policies via options pattern
        services.ConfigureOptions<AuthorizationOptionsSetup>();

        return services;
    }
}
