namespace CarpetBG.Application.DTOs.Addresses;

public class AddressDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerFullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DisplayAddress { get; set; } = string.Empty;
}
