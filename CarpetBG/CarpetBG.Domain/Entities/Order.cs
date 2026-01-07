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
    public int? OrderBy { get; set; }
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public OrderStatuses Status { get; set; }
    public string Note { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = [];
}
