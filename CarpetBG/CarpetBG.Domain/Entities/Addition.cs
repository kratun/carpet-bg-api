using CarpetBG.Domain.Enums;

namespace CarpetBG.Domain.Entities;

public class Addition : BaseEntity, IAddition
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public AdditionTypes AdditionType { get; set; }
    public decimal Value { get; set; }
    public Guid OrderItemId { get; set; }
    public virtual OrderItem OrderItem { get; set; } = null!;
}
