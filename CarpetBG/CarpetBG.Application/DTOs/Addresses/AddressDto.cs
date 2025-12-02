namespace CarpetBG.Application.DTOs.Addresses;

public class AddressDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DisplayAddress { get; set; } = string.Empty;
}
