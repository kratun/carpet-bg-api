namespace CarpetBG.Infrastructure.Options;

public sealed class AuthOptions
{
    public const string SectionName = "Auth0";

    public string Authority { get; init; } = default!;
    public string Audience { get; init; } = default!;
}
