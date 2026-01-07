using CarpetBG.Domain.Entities;

namespace CarpetBG.Application.DTOs.Customers;

public class CreateCustomerDto
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<Address> Addresses { get; set; } = [];
}
