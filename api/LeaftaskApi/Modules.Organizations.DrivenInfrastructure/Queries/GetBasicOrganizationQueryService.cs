using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Application.Management;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Queries;

public sealed class GetOrganizationDetailsQueryService(OrganizationDbContext dbContext)
    : IGetOrganizationDetailsQueryService
{
    public async Task<BasicOrganizationResponse?> GetOrganizationDetailsAsync(Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Organizations
            .AsNoTracking()
            .Where(organization => organization.Id == id)
            .Select(organization => new BasicOrganizationResponse(
                organization.Id,
                organization.Name,
                organization.Description,
                organization.Website,
                organization.Invitations.Count(invitation => invitation.Status == InvitationStatus.Accepted),
                0,
                0,
                organization.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
}
