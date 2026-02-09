using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.DTOs.Orders;

public class OrderDto
{
    public Guid Id { get; set; }
    public long OrderNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsExpress { get; set; }
    public string CustomerFullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PickupAddress { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid PickupAddressId { get; set; }
    public string? PickupTimeRange { get; set; }
    public DateTime? PickupDate { get; set; }
    public Guid? DeliveryAddressId { get; set; }
    public string DeliveryTimeRange { get; set; } = string.Empty;
    public DateTime? DeliveryDate { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public OrderStatuses Status { get; set; }
    public int? OrderBy { get; set; }
    public string Note { get; set; } = string.Empty;
    public List<OrderItemDto> OrderItems { get; set; } = [];
    public decimal TotalAmount => OrderItems.Sum(i => i.Amount);
}
