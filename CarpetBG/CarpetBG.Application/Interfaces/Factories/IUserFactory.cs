using CarpetBG.Application.DTOs.Users;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Factories;

public interface IUserFactory
{
    User CreateFromDto(CreateUserDto dto);
}
