using CarpetBG.Application.DTOs.Common;
using CarpetBG.Application.Enums;

namespace CarpetBG.Application.DTOs.Orders;

public class OrderFilterDto : BaseFilter<OrderFilter>
{

    public OrderSortBy SortBy { get; set; } = OrderSortBy.CreatedAt;
    public SortDirection SortDirection { get; set; } = SortDirection.Desc;
}
