namespace CarpetBG.Application.DTOs.Users;

public sealed record UserMeDto(
    Guid Id,
    string Email,
    IReadOnlyList<string> Roles
);
