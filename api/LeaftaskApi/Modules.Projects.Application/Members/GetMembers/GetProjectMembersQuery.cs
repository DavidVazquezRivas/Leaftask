using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Members.GetMembers;

public sealed class GetProjectMembersQuery : IPaginatedQuery<Result<PaginatedResult<ProjectMemberDto>>>
{
    public required Guid ProjectId { get; init; }
    public int Limit { get; init; } = 10;
    public string? Cursor { get; init; }
    public IReadOnlyCollection<string> Sort { get; init; } = [];
}
