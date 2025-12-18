using CarpetBG.Application.DTOs.Common;
using CarpetBG.Application.Enums;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.DTOs.Orders;

public class OrderFilterDto : BaseFilter
{
    public List<OrderStatuses> Statuses { get; set; } = [];
    public DateTime? PickupDate { get; set; }
    public DateTime? DeliveryDate { get; set; }

    public OrderSortBy SortBy { get; set; } = OrderSortBy.CreatedAt;
    public SortDirection SortDirection { get; set; } = SortDirection.Desc;
}
