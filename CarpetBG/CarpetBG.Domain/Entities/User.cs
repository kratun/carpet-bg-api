using CarpetBG.Domain.Entities;

public class User : BaseEntity
{
    private readonly List<UserRole> _roles = new();

    public IReadOnlyCollection<UserRole> Roles => _roles;

    public string Email { get; set; } = string.Empty;

    public Customer? Customer { get; set; }

    protected User() { }

    public User(string email)
    {
        Id = Guid.NewGuid();
        Email = email;
    }

    public void AddRole(Guid roleId)
    {
        _roles.Add(new UserRole
        {
            UserId = Id,
            RoleId = roleId
        });
    }
}
