namespace CarpetBG.Application.DTOs.Orders;

public class OrderDeliveryConfirmDto
{
    public decimal PaidAmount { get; set; }
    public List<Guid> DeliveredItems { get; set; } = [];
}
