using CarpetBG.Application.DTOs.Users;
using CarpetBG.Domain.Entities;
using CarpetBG.Shared;

namespace CarpetBG.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result<User>> CreateUserAsync(CreateUserDto dto);
}
