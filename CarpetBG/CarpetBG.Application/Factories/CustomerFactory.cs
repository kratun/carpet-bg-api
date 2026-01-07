using CarpetBG.Application.DTOs.Customers;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Factories;

public class CustomerFactory : BaseFactory, ICustomerFactory
{
    public Customer CreateFromDto(CreateCustomerDto dto)
    {
        return new()
        {
            FullName = dto.FullName,
            Addresses = dto.Addresses,
            PhoneNumber = dto.PhoneNumber
        };
    }

    public CustomerDto CreateFromEntity(Customer entity)
    {
        throw new NotImplementedException();
    }
}
