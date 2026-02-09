using System.Security.Claims;

namespace CarpetBG.Infrastructure.Authentication.Claims;

internal static class EmailClaimResolver
{
    private static readonly string[] EmailClaimTypes =
    {
        ClaimTypes.Email,
        "email",
        "https://api.yourapp.com/email",
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
    };

    public static string? Resolve(ClaimsIdentity identity) =>
        EmailClaimTypes
            .Select(identity.FindFirst)
            .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c?.Value))
            ?.Value;
}
