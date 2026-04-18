using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Members.GetMembers;

public sealed class GetOrganizationMembersQuery : IPaginatedQuery<Result<PaginatedResult<OrganizationMemberDto>>>
{
    public Guid OrganizationId { get; init; }
    public int Limit { get; init; } = 10;
    public string? Cursor { get; init; }
    public IReadOnlyCollection<string> Sort { get; init; } = [];
}
