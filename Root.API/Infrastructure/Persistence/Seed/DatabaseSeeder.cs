using Microsoft.EntityFrameworkCore;
using Root.API.Application.Abstractions;
using Root.API.Domain.Constants;
using Root.API.Domain.Entities;
using Root.API.Infrastructure.Persistence;

namespace Root.API.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        await SeedRolesAsync(context, logger);
        await SeedDefaultAdminAsync(context, passwordHasher, logger);
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context, ILogger logger)
    {
        var existingRoleIds = await context.Roles.Select(r => r.Id).ToListAsync();

        var rolesToSeed = new[]
        {
            new { Id = RoleSeeds.UserRoleId, Name = RoleSeeds.UserRoleName },
            new { Id = RoleSeeds.AdminRoleId, Name = RoleSeeds.AdminRoleName },
            new { Id = RoleSeeds.AgentRoleId, Name = RoleSeeds.AgentRoleName }
        };

        foreach (var seed in rolesToSeed)
        {
            if (!existingRoleIds.Contains(seed.Id))
            {
                context.Roles.Add(new Role(seed.Id, seed.Name));
                logger.LogInformation("Seeding role: {RoleName}", seed.Name);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedDefaultAdminAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger logger)
    {
        var adminExists = await context.Users.AnyAsync(u => u.Id == RoleSeeds.DefaultAdminUserId);
        if (adminExists)
            return;

        var usernameExists = await context.Users
            .AnyAsync(u => u.Username == RoleSeeds.DefaultAdminUsername);

        if (usernameExists)
        {
            logger.LogWarning(
                "Default admin username '{Username}' already exists under a different ID; skipping seed.",
                RoleSeeds.DefaultAdminUsername);
            return;
        }

        var passwordHash = passwordHasher.Hash(RoleSeeds.DefaultAdminPassword);
        var admin = new User(
            RoleSeeds.DefaultAdminName,
            RoleSeeds.DefaultAdminUsername,
            passwordHash,
            RoleSeeds.AdminRoleId);

        // Set deterministic ID via reflection to keep seed idempotent
        typeof(User)
            .GetProperty(nameof(User.Id))!
            .SetValue(admin, RoleSeeds.DefaultAdminUserId);

        context.Users.Add(admin);
        await context.SaveChangesAsync();

        logger.LogInformation("Default admin account seeded (username: '{Username}').", RoleSeeds.DefaultAdminUsername);
    }
}
