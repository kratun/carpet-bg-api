using CarpetBG.Domain.Enums;

namespace CarpetBG.Domain.Entities;

public class Order : BaseEntity
{
    public Guid PickupAddressId { get; set; }
    public virtual Address PickupAddress { get; set; } = null!;
    public string? PickupTimeRange { get; set; } = null!;
    public DateTime? PickupDate { get; set; }

    public Guid? DeliveryAddressId { get; set; }
    public virtual Address DeliveryAddress { get; set; } = null!;
    public string DeliveryTimeRange { get; set; } = string.Empty;
    public DateTime? DeliveryDate { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public OrderStatuses Status { get; set; }
    public string Note { get; set; } = string.Empty;
    public bool IsExpress { get; set; }
    public decimal PercentagePriceAddition { get; set; }
    public List<OrderItem> Items { get; set; } = [];
}
