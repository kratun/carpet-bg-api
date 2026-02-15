namespace CarpetBG.Application.DTOs.Orders;

public class OrderPrintDto
{
    public string OrderNumber { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public bool IsExpress { get; set; }
    public string CustomerFullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PickupAddress { get; set; } = string.Empty;
    public string? PickupTimeRange { get; set; }
    public string PickupDate { get; set; } = string.Empty;
    public string DeliveryTimeRange { get; set; } = string.Empty;
    public string DeliveryDate { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public List<OrderPrintItemDto> OrderItems { get; set; } = [];
    public string TotalAmount { get; set; } = string.Empty;
    public string TotalQuantity { get; set; } = string.Empty;
}
