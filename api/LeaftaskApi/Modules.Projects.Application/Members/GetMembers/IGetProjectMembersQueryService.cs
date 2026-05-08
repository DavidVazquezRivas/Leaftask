using BuildingBlocks.Application.Queries;

namespace Modules.Projects.Application.Members.GetMembers;

public interface IGetProjectMembersQueryService
{
    Task<PaginatedResult<ProjectMemberDto>> GetMembersAsync(
        Guid projectId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default);
}
