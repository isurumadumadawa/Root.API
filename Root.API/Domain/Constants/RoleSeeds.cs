namespace Root.API.Domain.Constants;

public static class RoleSeeds
{
    public static readonly Guid UserRoleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid AdminRoleId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    public static readonly Guid AgentRoleId = Guid.Parse("00000000-0000-0000-0000-000000000003");
    public static readonly Guid DefaultAdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000010");

    public const string UserRoleName = "user";
    public const string AdminRoleName = "admin";
    public const string AgentRoleName = "agent";

    public const string DefaultAdminUsername = "Root Admin";
    public const string DefaultAdminName = "Root Admin";
    public const string DefaultAdminPassword = "123@Admin";
}
