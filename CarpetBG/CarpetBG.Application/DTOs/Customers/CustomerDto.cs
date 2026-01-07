using CarpetBG.Application.DTOs.Addresses;

namespace CarpetBG.Application.DTOs.Customers;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public AddressDto Address { get; set; } = new();
}
