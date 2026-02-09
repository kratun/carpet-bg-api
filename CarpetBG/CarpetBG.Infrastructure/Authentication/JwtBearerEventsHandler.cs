using System.Security.Claims;

using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Infrastructure.Authentication.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CarpetBG.Infrastructure.Authentication;

internal sealed class JwtBearerEventsHandler : JwtBearerEvents
{
    private readonly IUserService _userService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<JwtBearerEventsHandler> _logger;

    public JwtBearerEventsHandler(
        IUserService userService,
        IMemoryCache cache,
        ILogger<JwtBearerEventsHandler> logger)
    {
        _userService = userService;
        _cache = cache;
        _logger = logger;
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        if (context.Principal?.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            return;

        var email = EmailClaimResolver.Resolve(identity);

        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("JWT validated but email claim missing");
            return;
        }

        var cacheKey = $"user_roles:{email}";
        if (!_cache.TryGetValue(cacheKey, out List<string>? roles))
        {
            var result = await _userService.GetUserRolesAsync(email);
            roles = result.Value;
            _cache.Set(cacheKey, roles, TimeSpan.FromMinutes(10));
        }

        if (roles is null) return;

        foreach (var role in roles)
        {
            if (!identity.HasClaim(ClaimTypes.Role, role))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
        }
    }
}
