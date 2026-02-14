using CarpetBG.Domain.Enums;

namespace CarpetBG.Domain.Entities;

public class OrderItem : BaseEntity
{
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public decimal Price { get; set; }
    public string Note { get; set; } = string.Empty;
    // TODO Add ExpressServicePrice to ensure if the express price has been changed in future
    // public decimal ExpressServicePrice { get; set; }
    // TODO Add ProductName to ensure if the name has been changed in future
    // public string ProductName { get; set; } = string.Empty;
    // TODO Add ProductNormalizedName to ensure if the name has been changed in future
    // public string ProductNormalizedName { get; set; } = string.Empty;
    public bool IsDelivered { get; set; } = false;
    public OrderItemStatuses Status { get; set; }
    public List<Addition> Additions { get; set; } = [];
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public Guid? ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}
