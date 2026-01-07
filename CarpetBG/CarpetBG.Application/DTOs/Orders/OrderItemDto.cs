using CarpetBG.Application.DTOs.Additions;
using CarpetBG.Application.Helpers;
using CarpetBG.Domain.Enums;

namespace CarpetBG.Application.DTOs.Orders;

public class OrderItemDto
{
    public Guid? Id { get; set; }
    public Guid ProductId { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public bool IsSquare => Width.HasValue && !Height.HasValue;
    public decimal Price { get; set; }
    public string Note { get; set; } = string.Empty;
    public OrderItemStatuses Status { get; set; }
    public List<AdditionDto> Additions { get; set; } = [];
    public decimal Amount => OrderItemHelper.CalculateAmount(Price, Width, Height, Additions);
}
