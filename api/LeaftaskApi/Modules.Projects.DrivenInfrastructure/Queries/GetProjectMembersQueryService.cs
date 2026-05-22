using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Application.Members.GetMembers;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Queries;

public sealed class GetProjectMembersQueryService(ProjectsDbContext dbContext) : IGetProjectMembersQueryService
{
    private const string IdSort = "id:asc";

    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<MemberRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<MemberRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", row => row.Id, value => Guid.Parse(value), value => ((Guid)value).ToString("D")),
            ["firstName"] = new("firstName", row => row.FirstName, value => value, value => (string)value),
            ["email"] = new("email", row => row.Email ?? string.Empty, value => value, value => (string)value)
        };

    private static readonly IReadOnlyList<string> DefaultSort = ["firstName:asc", IdSort];

    public async Task<PaginatedResult<ProjectMemberDto>> GetMembersAsync(
        Guid projectId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default)
    {
        List<MemberRow> members = await (
            from m in dbContext.ProjectMembers.AsNoTracking()
            join u in dbContext.UserReadModels.AsNoTracking() on m.MemberId equals u.Id into userGroup
            from u in userGroup.DefaultIfEmpty()
            where EF.Property<Guid>(m, "project_id") == projectId
            select new MemberRow(
                m.MemberId,
                u != null ? u.FirstName : "Agent",
                u != null ? u.LastName : string.Empty,
                u != null ? u.Email : null,
                EF.Property<Guid>(m, "project_role_id")))
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<string> effectiveSort = NormalizeSort(sort);

        return CursorPaginationHelper.Paginate(
            members,
            limit,
            cursor,
            effectiveSort,
            SortFields,
            DefaultSort,
            row => new ProjectMemberDto(row.Id, $"{row.FirstName} {row.LastName}".Trim(), row.Email, row.RoleId));
    }

    private static IReadOnlyCollection<string> NormalizeSort(IReadOnlyCollection<string> sort) =>
        sort.Count == 0 || sort.Any(s => s.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            ? sort
            : [.. sort, IdSort];

    private sealed record MemberRow(Guid Id, string FirstName, string LastName, string? Email, Guid RoleId);
}
