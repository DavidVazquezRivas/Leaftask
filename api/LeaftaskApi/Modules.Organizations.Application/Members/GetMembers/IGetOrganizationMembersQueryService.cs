using BuildingBlocks.Application.Queries;

namespace Modules.Organizations.Application.Members.GetMembers;

public interface IGetOrganizationMembersQueryService
{
    Task<PaginatedResult<OrganizationMemberDto>> GetOrganizationMembersAsync(
        Guid organizationId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default);
}
