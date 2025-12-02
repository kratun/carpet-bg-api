using CarpetBG.Application.Enums;
using CarpetBG.Domain.Constants;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.DTOs.Orders;

public class OrderFilterDto
{
    public string? SearchTerm { get; set; }
    public List<OrderStatuses> Statuses { get; set; } = [];
    public DateTime? PickupDate { get; set; }
    public DateTime? DeliveryDate { get; set; }

    public OrderSortBy SortBy { get; set; } = OrderSortBy.CreatedAt;
    public SortDirection SortDirection { get; set; } = SortDirection.Desc;

    public int PageNumber { get; set; } = PaginationConstants.DefaultPageNumber;
    public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
}
