using Root.API.Domain.Enums;

namespace Root.API.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? Position { get; private set; }
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private User() { } // EF Core

    public User(string name, string username, string passwordHash, Guid roleId, string? position = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Username = username;
        PasswordHash = passwordHash;
        Position = position;
        RoleId = roleId;
        CreatedAtUtc = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void Update(string name, string? position, Guid? roleId = null)
    {
        Name = name;
        Position = position;
        if (roleId.HasValue)
            RoleId = roleId.Value;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public UserStatus Status => IsDeleted ? UserStatus.Deleted : UserStatus.Active;
}
