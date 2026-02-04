namespace CarpetBG.Infrastructure.Data;

public sealed class PostgreOptions
{
    public const string SectionName = "Postgre";

    public string DbName { get; init; } = default!;
    public string DefaultConnection { get; init; } = default!;
}
