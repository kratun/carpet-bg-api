namespace CarpetBG.Domain.Entities;

public class Address : BaseEntity
{
    public string DisplayAddress { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
