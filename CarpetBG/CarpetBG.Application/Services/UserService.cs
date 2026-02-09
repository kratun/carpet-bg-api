using System.Net;

using CarpetBG.Application.DTOs.Users;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Shared;

using Microsoft.Extensions.Caching.Memory;

namespace CarpetBG.Application.Services;

public class UserService(IUserRepository userRepository, IMemoryCache cache) : IUserService
{
    public async Task<Result<bool>> AddUserRoleAsync(string email, string roleName)
    {
        var user = await userRepository.GetByEmailAsync(email);
        var hasUser = true;
        if (user == null)
        {
            hasUser = false;
            user = new User(email);
        }

        // TODO replace with roleRepository.GetByRoleNameAsync();
        var roleId = Guid.Parse("968decd6-2df4-48f9-96dd-92e2fb7cd1cf");
        user.AddRole(roleId);
        var isSuccess = false;
        var successResult = Result<bool>.Success(true);
        var failureResult = Result<bool>.Failure("Something went wrong on adding user role", HttpStatusCode.InternalServerError);
        if (!hasUser)
        {
            var addedUser = await userRepository.AddAsync(user);
            isSuccess = addedUser != null;

            return isSuccess ? successResult : failureResult;
        }

        var userRole = user.Roles.First(r => r.RoleId == roleId);
        var addedRole = await userRepository.AddUserRoleAsync(userRole);
        isSuccess = addedRole != null;
        return isSuccess ? successResult : failureResult;
    }

    public async Task<Result<UserMeDto>> GetCurrentUserAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);

        if (user is null)
        {
            return Result<UserMeDto>.Failure("User not found.");
        }

        var result = new UserMeDto(
            user.Id,
            user.Email,
            [.. user.Roles.Select(r => r.Role.Name)]
        );

        return Result<UserMeDto>.Success(result);
    }

    public async Task<Result<List<string>>> GetUserRolesAsync(string email)
    {
        var roles = await cache.GetOrCreateAsync(
            $"roles:{email}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                return await userRepository.GetUserRolesByEmailAsync(email);
            }) ?? [];
        return Result<List<string>>.Success(roles);
    }
}
