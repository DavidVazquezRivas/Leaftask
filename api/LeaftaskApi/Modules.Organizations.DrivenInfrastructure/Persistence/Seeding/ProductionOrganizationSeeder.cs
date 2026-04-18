using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.DrivenInfrastructure.Persistence.Seeding;

public sealed class ProductionOrganizationSeeder : IOrganizationSeederStrategy
{
    private static readonly OrganizationPermission[] Permissions =
    [
        new("Configure Organization", "Modify organization settings, branding, and general configuration"),
        new("Invite Members", "Send invitations to new members to join the organization"),
        new("Remove Members", "Remove existing members from the organization"),
        new("Create Projects", "Create new projects within the organization"),
        new("Configure Projects", "Modify project settings and configurations"),
        new("Delete Projects", "Remove projects from the organization")
    ];

    public async Task SeedAsync(OrganizationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.OrganizationPermissions.AnyAsync(cancellationToken))
        {
            return;
        }

        await dbContext.OrganizationPermissions.AddRangeAsync(Permissions, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
