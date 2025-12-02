using CarpetBG.Domain.Enums;

namespace CarpetBG.Domain.Entities;

public class OrderItemAddition : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public AdditionTypes AdditionType { get; set; }
    public decimal Value { get; set; }
    public Guid AdditionId { get; set; }
    public virtual Addition Addition { get; set; } = null!;
    public Guid OrderItemId { get; set; }
    public virtual OrderItem OrderItem { get; set; } = null!;
}
