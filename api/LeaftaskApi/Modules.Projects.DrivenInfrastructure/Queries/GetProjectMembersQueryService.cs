using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Application.Members.GetMembers;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Queries;

public sealed class GetProjectMembersQueryService(ProjectsDbContext dbContext) : IGetProjectMembersQueryService
{
    private const string IdSort = "id:asc";

    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<MemberRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<MemberRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", row => row.Id, value => Guid.Parse(value), value => ((Guid)value).ToString("D")),
            ["firstName"] = new("firstName", row => row.DisplayName, value => value, value => (string)value),
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
        List<RawMemberRow> rawRows = await (
            from m in dbContext.ProjectMembers.AsNoTracking()
            join u in dbContext.UserReadModels.AsNoTracking()
                on m.MemberId equals u.Id into userGroup
            from u in userGroup.DefaultIfEmpty()
            join a in dbContext.AgentReadModels.AsNoTracking()
                on m.MemberId equals a.Id into agentGroup
            from a in agentGroup.DefaultIfEmpty()
            where EF.Property<Guid>(m, "project_id") == projectId
            select new RawMemberRow(
                m.MemberId,
                m.MemberType,
                u != null ? u.FirstName : null,
                u != null ? u.LastName : null,
                u != null ? u.Email : null,
                a != null ? a.Name : null,
                EF.Property<Guid>(m, "project_role_id")))
            .ToListAsync(cancellationToken);

        List<MemberRow> members = rawRows.Select(r => r.MemberType == MemberType.Agent
            ? new MemberRow(r.Id, r.AgentName ?? "Agent", string.Empty, null, r.RoleId, MemberType.Agent)
            : new MemberRow(r.Id, r.UserFirstName ?? string.Empty, r.UserLastName ?? string.Empty, r.UserEmail, r.RoleId, MemberType.User))
            .ToList();

        IReadOnlyCollection<string> effectiveSort = NormalizeSort(sort);

        return CursorPaginationHelper.Paginate(
            members,
            limit,
            cursor,
            effectiveSort,
            SortFields,
            DefaultSort,
            row => new ProjectMemberDto(row.Id, row.DisplayName, row.Email, row.RoleId, row.MemberType == MemberType.Agent ? "agent" : "person"));
    }

    private static IReadOnlyCollection<string> NormalizeSort(IReadOnlyCollection<string> sort) =>
        sort.Count == 0 || sort.Any(s => s.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            ? sort
            : [.. sort, IdSort];

    private sealed record RawMemberRow(
        Guid Id,
        MemberType MemberType,
        string? UserFirstName,
        string? UserLastName,
        string? UserEmail,
        string? AgentName,
        Guid RoleId);

    private sealed record MemberRow(Guid Id, string FirstName, string LastName, string? Email, Guid RoleId, MemberType MemberType)
    {
        public string DisplayName => string.IsNullOrWhiteSpace(LastName)
            ? FirstName
            : $"{FirstName} {LastName}".Trim();
    }
}
