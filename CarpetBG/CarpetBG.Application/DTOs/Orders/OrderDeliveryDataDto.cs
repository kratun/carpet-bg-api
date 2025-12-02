namespace CarpetBG.Application.DTOs.Orders;

public class OrderDeliveryDataDto
{
    public Guid DeliveryAddressId { get; set; }
    public string DisplayAddress { get; set; } = string.Empty;
    public string DeliveryTimeRange { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public string Note { get; set; } = string.Empty;
}
