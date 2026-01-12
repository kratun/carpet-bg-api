using CarpetBG.Application.DTOs.Customers;
using CarpetBG.Domain.Entities;
using CarpetBG.Shared;

namespace CarpetBG.Application.Interfaces.Services;

public interface ICustomerService
{
    Task<Result<Customer>> CreateCustomerAsync(CreateCustomerDto dto);
}
