
namespace CarpetBG.Application.DTOs.Orders;

public class CreateOrderDto
{
    public Guid CustomerId { get; set; }
    public Guid PickupAddressId { get; set; }
    public string PickupTimeRange { get; set; } = string.Empty;
    public DateTime PickupDate { get; set; }
    public string Note { get; set; } = string.Empty;
    public bool IsExpress { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = [];
}
