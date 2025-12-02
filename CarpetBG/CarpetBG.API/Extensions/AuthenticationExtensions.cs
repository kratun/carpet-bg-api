using Microsoft.AspNetCore.Authorization;

namespace CarpetBG.API.Extensions;

public static class AuthenticationExtensions
{
    public const string MyPolicyName = "my-policy-name";
    public const string MyPolicyClaimName = "my-policy-claim-name";
    public const string MyPolicyClaimValue = "my-policy-claim-value";
    public static IServiceCollection AddAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        //    {
        //        options.Authority = configuration["AUTH_AUTHORITY"];
        //        options.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidIssuer = configuration["AUTH_AUDIENCE"],
        //            ValidAudience = configuration["AUTH_AUDIENCE"],

        //            ValidateLifetime = true,
        //            SaveSigninToken = true,
        //        };

        //        options.IncludeErrorDetails = true;
        //        options.UseSecurityTokenValidators = true;
        //    });

        //services.AddAuthorization(ConfigureAuthorization);

        return services;
    }

    private static void ConfigureAuthorization(AuthorizationOptions options)
    {
        //options.AddPolicy(MyPolicyName, policy =>
        //{
        //    policy.RequireAuthenticatedUser();
        //    policy.RequireClaim(MyPolicyClaimName, MyPolicyClaimValue);
        //});
    }
}
