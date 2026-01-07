using CarpetBG.Application.DTOs.Customers;
using CarpetBG.Application.Interfaces.Common;
using CarpetBG.Application.Interfaces.Factories;
using CarpetBG.Application.Interfaces.Repositories;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Domain.Entities;
using CarpetBG.Shared;

namespace CarpetBG.Application.Services;

public class CustomerService(ICustomerRepository customerRepository, ICustomerFactory customerFactory, IValidator<CreateCustomerDto> customerValidator) : ICustomerService
{
    public async Task<Result<Customer>> CreateCustomerAsync(CreateCustomerDto dto)
    {
        var error = customerValidator.Validate(dto);
        if (error != null)
        {
            return Result<Customer>.Failure(error);
        }

        var customer = customerFactory.CreateFromDto(dto);

        await customerRepository.AddAsync(customer);

        return Result<Customer>.Success(customer);
    }
}
