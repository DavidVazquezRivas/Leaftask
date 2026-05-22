using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Repositories;

public class OrganizationRepository(OrganizationDbContext dbContext) : IOrganizationRepository
{
    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Organizations
            .AsSplitQuery()
            .Include(organization => organization.Invitations)
            .Include(organization => organization.Roles)
                .ThenInclude(role => role.Permissions)
            .FirstOrDefaultAsync(organization => organization.Id == id, cancellationToken);

    public async Task AddAsync(Organization organization, CancellationToken cancellationToken = default) =>
        await dbContext.AddAsync(organization, cancellationToken);

    public async Task AddRoleAsync(OrganizationRole role, CancellationToken cancellationToken = default) =>
        await dbContext.AddAsync(role, cancellationToken);

    public async Task AddInvitationAsync(OrganizationInvitation invitation, CancellationToken cancellationToken = default) =>
        await dbContext.AddAsync(invitation, cancellationToken);

    public Task RemoveAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        dbContext.Remove(organization);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}

public class OrganizationPermissionRepository(OrganizationDbContext dbContext) : IOrganizationPermissionRepository
{
    public async Task<IReadOnlyCollection<OrganizationPermission>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.OrganizationPermissions.AsNoTracking().ToListAsync(cancellationToken);
}
