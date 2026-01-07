namespace CarpetBG.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public List<UserRole> UserRoles { get; set; } = [];
    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }
}
