namespace CarpetBG.Domain.Entities;

public class Address : BaseEntity
{
    public string DisplayAddress { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public virtual User User { get; set; }
}
