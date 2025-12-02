using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.DTOs.Orders;

public class UpdateOrderStatusDto
{
    public OrderStatuses NextStatus { get; set; }
}
