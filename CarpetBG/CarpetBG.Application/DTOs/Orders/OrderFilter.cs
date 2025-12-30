using CarpetBG.Application.DTOs.Common;

namespace CarpetBG.Application.DTOs.Orders;

public class OrderFilter
{
    public List<StatusFilter> Statuses { get; set; } = [];

    public DateTime? PickupDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
}
