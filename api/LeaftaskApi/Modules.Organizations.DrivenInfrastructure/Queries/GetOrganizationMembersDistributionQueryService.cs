using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Application.Members.GetDistribution;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Queries;

public sealed class GetOrganizationMembersDistributionQueryService(OrganizationDbContext dbContext)
    : IGetOrganizationMembersDistributionQueryService
{
    public async Task<IReadOnlyList<OrganizationMemberDistributionDto>> GetOrganizationMembersDistributionAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        List<Guid> roleIds = await dbContext.OrganizationRoles
            .AsNoTracking()
            .Where(role => role.OrganizationId == organizationId)
            .OrderBy(role => role.Name)
            .Select(role => role.Id)
            .ToListAsync(cancellationToken);

        if (roleIds.Count == 0)
        {
            return [];
        }

        Dictionary<Guid, int> memberCounts = await dbContext.OrganizationInvitations
            .AsNoTracking()
            .Where(invitation => invitation.OrganizationId == organizationId && invitation.Status == InvitationStatus.Accepted)
            .GroupBy(invitation => invitation.OrganizationRoleId)
            .Select(group => new { RoleId = group.Key, MemberCount = group.Count() })
            .ToDictionaryAsync(group => group.RoleId, group => group.MemberCount, cancellationToken);

        List<OrganizationMemberDistributionDto> distribution = [];
        foreach (Guid roleId in roleIds)
        {
            distribution.Add(new OrganizationMemberDistributionDto(
                roleId,
                memberCounts.TryGetValue(roleId, out int memberCount) ? memberCount : 0));
        }

        return distribution;
    }
}
