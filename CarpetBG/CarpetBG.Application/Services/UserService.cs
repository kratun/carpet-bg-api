using CarpetBG.Application.DTOs.Users;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Domain.Entities;
using CarpetBG.Shared;

namespace CarpetBG.Application.Services;

public class UserService(IUserRepository userRepository, IUserFactory userFactory, IValidator<CreateUserDto> userValidator) : IUserService
{
    public async Task<Result<User>> CreateUserAsync(CreateUserDto dto)
    {
        var error = userValidator.Validate(dto);
        if (error != null)
        {
            return Result<User>.Failure(error);
        }

        var user = userFactory.CreateFromDto(dto);

        await userRepository.AddAsync(user);

        return Result<User>.Success(user);
    }
}
