using CarpetBG.Domain.Enums;

namespace CarpetBG.Domain.Entities;

public class Addition : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public AdditionTypes AdditionType { get; set; }
    public decimal Value { get; set; }

    public virtual List<OrderItemAddition> OrderItemAdditions { get; set; } = [];
}
