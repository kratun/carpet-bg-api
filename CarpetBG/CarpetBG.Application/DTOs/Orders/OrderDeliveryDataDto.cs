namespace CarpetBG.Application.DTOs.Orders;

public class OrderDeliveryDataDto
{
    public Guid AddressId { get; set; }
    public string DisplayAddress { get; set; } = string.Empty;
    public string TimeRange { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Note { get; set; } = string.Empty;
}
