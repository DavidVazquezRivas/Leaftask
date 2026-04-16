using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Application.Management;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Queries;

public sealed class GetBasicOrganizationQueryService(OrganizationDbContext dbContext)
    : IGetBasicOrganizationQueryService
{
    public async Task<BasicOrganizationResponse?> GetBasicOrganizationsAsync(Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Organizations
            .AsNoTracking()
            .Select(organization => new BasicOrganizationResponse(id, organization.Name, organization.Description, 0, 0,
                0, organization.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
}
