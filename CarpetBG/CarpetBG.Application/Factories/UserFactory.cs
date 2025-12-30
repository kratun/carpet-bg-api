using CarpetBG.Application.DTOs.Users;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Factories;

public class UserFactory : BaseFactory, IUserFactory
{
    public User CreateFromDto(CreateUserDto dto)
    {
        return new()
        {
            FullName = dto.FullName,
            Addresses = dto.Addresses,
            PhoneNumber = dto.PhoneNumber
        };
    }

    public UserDto CreateFromEntity(User entity)
    {
        throw new NotImplementedException();
    }
}
