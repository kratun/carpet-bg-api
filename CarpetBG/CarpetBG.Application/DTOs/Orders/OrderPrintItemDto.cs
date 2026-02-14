namespace CarpetBG.Application.DTOs.Orders;

public class OrderPrintItemDto
{
    public Guid? Id { get; set; }
    public Guid ProductId { get; set; }
    public string Width { get; set; } = string.Empty;
    public string Height { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
    public string UnitPrice { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string ExpressServicePrice { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductNormalizedName { get; set; } = string.Empty;
    public string Measurment { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;

    public bool IsPlaceholder { get; init; }
}
