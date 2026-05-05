namespace Root.API.Domain.Entities;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public ICollection<User> Users { get; private set; } = new List<User>();

    private Role() { } // EF Core

    public Role(Guid id, string name)
    {
        Id = id;
        Name = name;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
