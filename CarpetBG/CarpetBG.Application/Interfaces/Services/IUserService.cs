using CarpetBG.Application.DTOs.Users;
using CarpetBG.Shared;

namespace CarpetBG.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result<UserMeDto>> GetCurrentUserAsync(string email);
    Task<Result<List<string>>> GetUserRolesAsync(string email);
    Task<Result<bool>> AddUserRoleAsync(string email, string roleName);
}
