using CarpetBG.Application.DTOs.Customers;
using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.Interfaces.Factories;

public interface ICustomerFactory : IBaseFactory
{
    Customer CreateFromDto(CreateCustomerDto dto);
}
